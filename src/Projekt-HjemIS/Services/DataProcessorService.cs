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

        private static Dictionary<string, int[]> _recordTypes = new Dictionary<string, int[]>
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

        private static List<RecordTypeAKTVEJ> _aktvejRecords = new List<RecordTypeAKTVEJ>();
        private static List<RecordTypeOther> _otherRecords = new List<RecordTypeOther>();

        public async Task ProcessData()
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

                        if (recordType != "000" && recordType != "999")
                        {
                            var record = ParseRecord(currentLine, recordType);

                            switch (record[0])
                            {
                                case "001":
                                    _aktvejRecords.Add(new RecordTypeAKTVEJ(record));
                                    break;
                                case "002":
                                case "005":
                                case "000":
                                case "999":
                                    break;
                                default:
                                    _otherRecords.Add(new RecordTypeOther(record));
                                    break;
                            }
                        }
                    }

                    var timer1 = new Stopwatch();
                    timer1.Start();

                    // upsert all parsed Record object to database
                    await SendRecordsToStagingTables();

                    timer1.Stop();
                    Debug.WriteLine(timer1.Elapsed);

                    var timer2 = new Stopwatch();
                    timer2.Start();

                    await BuildLocations();

                    timer2.Stop();
                    Debug.WriteLine(timer2.Elapsed);
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
        private static async Task BuildLocations()
        {
            var query = "INSERT INTO [location] (StreetName, CountyCode, StreetCode, HouseNumberFrom, HouseNumberTo, EvenOdd, PostalCode) " +
                        "SELECT AKTVEJ.StreetName, Other.CountyCode, Other.StreetCode, Other.HouseNumberFrom, Other.HouseNumberTo, Other.EvenOdd, Other.PostalCode " +
                        "FROM record_type_aktvej AS AKTVEJ " +
                        "JOIN record_type_other AS Other " +
                        "ON AKTVEJ.CountyCode = Other.CountyCode " +
                        "AND AKTVEJ.StreetCode = Other.StreetCode " +
                        "WHERE AKTVEJ.CountyCode = Other.CountyCode " +
                        "AND AKTVEJ.StreetCode = Other.StreetCode " +
                        "AND Other.PostalCode != '';";

            await databaseHandler.AddData(query);
        }

        private async Task SendRecordsToStagingTables()
        {
            var otherDt = _otherRecords.ToDataTable();
            await databaseHandler.AddBulkData<RecordTypeOther>(otherDt, "staging_record_type_other");
            
            var aktvejDt = _aktvejRecords.ToDataTable();
            await databaseHandler.AddBulkData<RecordTypeAKTVEJ>(aktvejDt, "staing_record_type_aktvej");
        }

        private async Task FinalizeSavingRecords()
        {

        }

        private static async Task UploadAktvej(Record record)
        {
            var convertedRecord = (RecordTypeAKTVEJ)record;

            var query = "INSERT INTO RecordTypeAKTVEJ " +
                        "(RecordType, CountyCode, StreetCode, [Timestamp], " +
                        "ToCountyCode, ToStreetCode, FromCountyCode, FromStreetCode, " +
                        "ThereStart, StreetAddrName, StreetName) " +
                            "SELECT @RecordType, @CountyCode, @StreetCode, @Timestamp, " +
                            "@ToCountyCode, @ToStreetCode, @FromCountyCode, @FromStreetCode," +
                            " @ThereStart, @StreetAddrName, @StreetName " +
                            "WHERE NOT EXISTS ( " +
                            "SELECT 1 FROM RecordTypeAKTVEJ " +
                            "WHERE StreetName = @StreetName " +
                        ")";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@RecordType", convertedRecord.RecordType),
                new SqlParameter("@CountyCode", convertedRecord.CountyCode),
                new SqlParameter("@StreetCode", convertedRecord.StreetCode),
                new SqlParameter("@Timestamp", convertedRecord.Timestamp),
                new SqlParameter("@ToCountyCode", convertedRecord.ToCountyCode),
                new SqlParameter("@ToStreetCode", convertedRecord.ToStreetCode),
                new SqlParameter("@FromCountyCode", convertedRecord.FromCountyCode),
                new SqlParameter("@FromStreetCode", convertedRecord.FromStreetCode),
                new SqlParameter("@ThereStart", convertedRecord.ThereStart),
                new SqlParameter("@StreetAddrName", convertedRecord.StreetAddrName),
                new SqlParameter("@StreetName", convertedRecord.StreetName.Trim()),
            };

            await databaseHandler.AddData(query, parameters.ToArray());
        }

        private static async Task UploadOther(Record record)
        {
            var convertedRecord = (RecordTypeOther)record;

            var query = "INSERT INTO RecordTypeOther " +
                        "(RecordType, CountyCode, StreetCode, HouseNumberFrom, " +
                        "HouseNumberTo, EvenOdd, [Timestamp], PostalCode) " +
                        "SELECT @RecordType, @CountyCode, @StreetCode, @HouseNumberFrom, @HouseNumberTo, @EvenOdd, @Timestamp, @PostalCode " +
                        "WHERE NOT EXISTS ( " +
                            "SELECT 1 FROM RecordTypeOther " +
                            "WHERE CountyCode = @CountyCode " +
                            "AND StreetCode = @StreetCode " +
                            "AND HouseNumberFrom = @HouseNumberFrom " +
                            "AND HouseNumberTo = @HouseNumberTo " +
                            "AND EvenOdd = @EvenOdd " +
                        ")";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@RecordType", convertedRecord.RecordType),
                new SqlParameter("@CountyCode", convertedRecord.CountyCode),
                new SqlParameter("@StreetCode", convertedRecord.StreetCode),
                new SqlParameter("@HouseNumberFrom", convertedRecord.HouseNumberFrom),
                new SqlParameter("@HouseNumberTo", convertedRecord.HouseNumberTo),
                new SqlParameter("@EvenOdd", convertedRecord.EvenOdd),
                new SqlParameter("@Timestamp", convertedRecord.Timestamp),
            };
            
            if (convertedRecord.RecordType == "004")
            {
                parameters.Add(new SqlParameter("@PostalCode", convertedRecord.PostalCode));
            }
            else
            {
                parameters.Add(new SqlParameter("@PostalCode", ""));
            }

            await databaseHandler.AddData(query, parameters.ToArray());
        }

        private static string[] ParseRecord(string loadedRecord, string type)
        {
            // used to keep track of which section we're at in the .
            var readDigits = 0;

            var record = new string[_recordTypes[type].Length];

            for (int i = 0; i < record.Length; i++)
            {
                // building the record section by section
                foreach (var sectionLength in _recordTypes[type])
                {
                    record[i] = loadedRecord.Substring(readDigits, sectionLength);
                    readDigits += sectionLength;
                    i++;
                }
            }

            return record;
        }
    }
}
