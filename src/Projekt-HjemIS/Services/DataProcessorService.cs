using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Services
{
    public class DataProcessorService
    {
        private static readonly DatabaseHandler databaseHandler = new DatabaseHandler();

        private static readonly Dictionary<string, int[]> _recordTypes = new Dictionary<string, int[]>
        {
            // All records are made up of sections with a pre-assigned
            // max digits. The numbers in this dictionary represent
            // the max digits of each section in each record-type    
            { "000", new int[] { 3 } },
            { "001", new int[] { 3, 4, 4, 12, 4, 4, 4, 4, 12, 20, 40 } },
            { "002", new int[] { 3, 4, 4, 4, 4, 2, 4, 12, 1, 12, 12, 34 } },
            { "003", new int[] { 3, 4, 4, 4, 4, 1, 12, 34 } },
            { "004", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 20 } },
            { "005", new int[] { 3, 4, 4, 2, 40, 12, 12 } },
            { "006", new int[] { 3, 4, 4, 4, 4, 1, 12, 6, 30 } },
            { "007", new int[] { 3, 4, 4, 4, 4, 1, 12, 2, 4, 30 } },
            { "008", new int[] { 3, 4, 4, 4, 4, 1, 12, 1, 30 } },
            { "009", new int[] { 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "010", new int[] { 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "011", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 30 } },
            { "012", new int[] { 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "013", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 20 } },
            { "014", new int[] { 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "015", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 30 } },
            { "999", new int[] { 3 } }
        };

        private static readonly string[] _badRecordTypes = new string[]
        {
            "000",
            "002",
            "005",
            "999"
        };

        private static List<RecordTypeAKTVEJ> _aktvejRecords = new List<RecordTypeAKTVEJ>();
        private static List<RecordTypeOther> _otherRecords = new List<RecordTypeOther>();

        public async Task ProcessDataAsync()
        {
            var path = "../../dropzone/full_data.txt";

            try
            {
                using (var reader = new StreamReader(path))
                {
                    var currentLine = "";
                    while ((currentLine = reader.ReadLine()) != null)
                    {
                        // all data (of correct format) starts with a record-type as it's 3 first digits.
                        var recordType = currentLine.Substring(0, 3);

                        if (!_badRecordTypes.Contains(recordType))
                        {
                            var record = ParseRecord(currentLine, recordType);

                            switch (record[0])
                            {
                                case "001":
                                    _aktvejRecords.Add(new RecordTypeAKTVEJ(record));
                                    break;
                                default:
                                    _otherRecords.Add(new RecordTypeOther(record));
                                    break;
                            }
                        }
                    }

                    // upsert all parsed Record object to database
                    await SendRecordsToStagingTablesAsync();

                    // merge record staging tables into a combined location table
                    await BuildLocationsAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Builds Location elements from already existing Record elements (in the form of RecordType- Aktvej/Other)
        /// and upserts them to database
        /// </summary>
        /// <returns></returns>
        private static async Task BuildLocationsAsync()
        {
            var query = "INSERT INTO [location] (StreetName, CountyCode, StreetCode, HouseNumberFrom, HouseNumberTo, EvenOdd, PostalCode) " +
                        "SELECT AKTVEJ.StreetName, Other.CountyCode, Other.StreetCode, Other.HouseNumberFrom, Other.HouseNumberTo, Other.EvenOdd, Other.PostalCode " +
                        "FROM staging_record_type_aktvej AS AKTVEJ " +
                        "JOIN staging_record_type_other AS Other " +
                        "ON AKTVEJ.CountyCode = Other.CountyCode " +
                        "AND AKTVEJ.StreetCode = Other.StreetCode " +
                        "WHERE AKTVEJ.CountyCode = Other.CountyCode " +
                        "AND AKTVEJ.StreetCode = Other.StreetCode " +
                        "AND Other.PostalCode != ''" +
                        "AND Other.PostalCode != '9999';";

            await databaseHandler.AddDataAsync(query);

            Debug.WriteLine("Locations done");
        }

        private async Task SendRecordsToStagingTablesAsync()
        {
            var otherDt = _otherRecords.ToDataTable();
            await databaseHandler.AddBulkData<RecordTypeOther>(otherDt, "staging_record_type_other");
            await FinalizeSavingOtherRecordsAsync();
            Debug.WriteLine("Others done");

            var aktvejDt = _aktvejRecords.ToDataTable();
            await databaseHandler.AddBulkData<RecordTypeAKTVEJ>(aktvejDt, "staging_record_type_aktvej");
            await FinalizeSavingAktvejRecordsAsync();
            Debug.WriteLine("Aktvej done");
        }

        private async Task FinalizeSavingOtherRecordsAsync()
        {
            var query = $"INSERT INTO record_type_other (CountyCode, StreetCode, HouseNumberFrom, HouseNumberTo, EvenOdd, PostalCode) " +
                        "SELECT DISTINCT staged.CountyCode, staged.StreetCode, staged.HouseNumberFrom, staged.HouseNumberTo, staged.EvenOdd, staged.PostalCode " +
                        $"FROM staging_record_type_other AS staged";

            await databaseHandler.AddDataAsync(query);
        }

        private async Task FinalizeSavingAktvejRecordsAsync()
        {
            var query = $"INSERT INTO record_type_aktvej (CountyCode, StreetCode, ToCountyCode, ToStreetCode, FromCountyCode, FromStreetCode, ThereStart, StreetAddrName, StreetName) " +
                $"SELECT DISTINCT CountyCode, StreetCode, ToCountyCode, ToStreetCode, FromCountyCode, FromStreetCode, ThereStart, StreetAddrName, StreetName " +
                $"FROM staging_record_type_aktvej";

            await databaseHandler.AddDataAsync(query);
        }

        public string[] ParseRecord(string loadedRecord, string type)
        {
            // used to keep track of which section we're at in the loaded record
            var readDigits = 0;

            var record = new string[_recordTypes[type].Length];

            for (int i = 0; i < record.Length; i++)
            {
                var sectionLength = _recordTypes[type][i];
                record[i] = loadedRecord.Substring(readDigits, sectionLength);
                readDigits += sectionLength;
            }

            return record;
        }
    }
}
