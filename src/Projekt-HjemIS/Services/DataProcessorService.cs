using Projekt_HjemIS.Models;
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
            { "000", new int[] { } },
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
            { "999", new int[] { } }
        };

        public async Task ProcessData()
        {
            var path = "../../dropzone/sample_data.txt";

            var timer = new Stopwatch();
            timer.Start();

            using (var reader = new StreamReader(path))
            {
                var records = new List<Record>();

                var currentLine = "";
                while ((currentLine = reader.ReadLine()) != null)
                {
                    // all data (of correct format) starts with a record-type as it's 3 first digits.
                    var recordType = currentLine.Substring(0, 3);

                    var record = ParseRecord(currentLine, recordType);

                    switch (record[0])
                    {
                        case "001":
                            records.Add(new RecordTypeAKTVEJ(record));
                            break;
                        case "002":
                            records.Add(new RecordTypeBOLIG());
                            break;
                        case "005":
                            records.Add(new RecordTypeNOTATVEJ());
                            break;
                        case "000":
                        case "999":
                            break;
                        default:
                            records.Add(new RecordTypeOther(record));
                            break;
                    }

                    Debug.WriteLine("");
                }

                // upsert all parsed Record object to database
                await SaveToDatabase(records);

                await BuildLocations();
            }

            timer.Stop();
            Debug.WriteLine(timer.Elapsed);
        }

        /// <summary>
        /// Builds Location elements from already existing Record elements (in the form of RecordType- Aktvej/Other)
        /// and upserts them to database
        /// </summary>
        /// <returns></returns>
        private static async Task BuildLocations()
        {
            var query = "INSERT INTO [Location] (StreetName, CountyCode, StreetCode, HouseNumberFrom, HouseNumberTo, EvenOdd, PostalCode) " +
                        "SELECT AKTVEJ.StreetName, Other.CountyCode, Other.StreetCode, Other.HouseNumberFrom, Other.HouseNumberTo, Other.EvenOdd, Other.PostalCode " +
                        "FROM RecordTypeAKTVEJ AS AKTVEJ " +
                        "JOIN RecordTypeOther AS Other " +
                        "ON AKTVEJ.CountyCode = Other.CountyCode " +
                        "AND AKTVEJ.StreetCode = Other.StreetCode " +
                        "WHERE AKTVEJ.CountyCode = Other.CountyCode " +
                        "AND AKTVEJ.StreetCode = Other.StreetCode " +
                        "AND Other.PostalCode != '';";

            await databaseHandler.AddData(query);
        }

        private static async Task SaveToDatabase(List<Record> records)
        {
            foreach (var record in records)
            {
                // upload logic moved into separate methods for better readability
                if (record.GetType() == typeof(RecordTypeAKTVEJ))
                {
                    await UploadAktvej(record);
                }
                else if (record.GetType() == typeof(RecordTypeOther))
                {
                    await UploadOther(record);
                }
            }
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
                new SqlParameter("@Timestamp", convertedRecord.Timestamp)
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
