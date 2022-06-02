using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems.Utility.Database_handling;

namespace Projekt_HjemIS.Systems
{
    /// <summary>
    /// Hovedforfatter: Kasper
    /// Handles record segmentation
    /// </summary>
    public static class RecordHandler
    {

        private static List<Location> _locationsList = new List<Location>();


        // Holds a single record.
        private static List<string> RecordSegments = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

        // Record segments
        private static int[] _postDist = new int[] { 3, 4, 4, 20, 20, 4, 4, 1, 4, 20 };

        /// <summary>
        /// Populate a list with data from a .txt file.
        /// </summary>

        /// <returns></returns>
        public static List<Location> GetRecords(string fileName)
        {
            // Keep count of how many record have been decoded and sent to the database.
            int recordCount = 0;

            Location currentLocation = new Location();
            Location prevLocation = new Location();

            using (StreamReader sr = new StreamReader(GetCurrentDirectory() + $@"\dropzone\{fileName}", Encoding.Default, true))

            {
                string currentLine = string.Empty;
                // Read each line from current file.
                while ((currentLine = sr.ReadLine()) != null)
                {
                    if (currentLine.Substring(0, 3) == "001")
                        BuildLocation(currentLocation, SpliceRecord(currentLine, _postDist));
                    switch (currentLine.Substring(0, 3))
                    {
                        case "001":
                            if (prevLocation.StreetCode != currentLocation.StreetCode)
                                _locationsList.Add(currentLocation);
                            break;
                        default:
                            break;
                    }
                    prevLocation = currentLocation;
                    currentLocation = new Location();

                    // Keep count of handled records and total records
                    if (currentLine.Substring(0, 3) == "999")
                    {
                        Debug.WriteLine(
                            "Amount of locations decoded: " + _locationsList.Count +
                            "\nTotal records: " + Int32.Parse(currentLine.Substring(4)) +
                            "\nAmount of handled records: " + (recordCount - 1));
                    }
                    recordCount++;
                }
                return _locationsList;
            }
        }

        /// <summary>
        /// Save a list of locations to a database.
        /// </summary>
        /// <param name="locations"></param>
        public async static Task<string> SaveRecords(List<Location> locations)
        {
            Dictionary<string, Location> loc = new Dictionary<string, Location>();

            
            DatabaseHandler dh = new DatabaseHandler();
            object records = await Task.Run(() => dh.CheckTable("Locations"));
            int numberOfRecords = (int)records;
            string result = string.Empty;
            DataTable dt = ListToDataTableConverter.ToDataTable(locations);
            if (numberOfRecords > 0)
            {
                int a = dh.UpdateBulkData(dt);
            }
            else { dh.AddBulkData<Location>(dt, "Locations"); }
            return "Indlæsning færdig";
        }

        /// <summary>
        /// Split a record into individual segments
        /// </summary>
        /// <param name="currentRecord"></param>
        /// <param name="recordType"></param>
        private static List<string> SpliceRecord(string currentRecord, int[] recordType)
        {
            int currentCol = 0;
            for (int i = 0; i < recordType.Length; i++)
            {
                if (currentRecord != null)
                {
                    RecordSegments[i] = currentRecord.Substring(currentCol, recordType[i]);
                    currentCol += recordType[i];
                }
            }
            return RecordSegments;
        }

        /// <summary>
        /// Fills a Location object with relevant data.
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="record"></param>
        private static void BuildLocation(Location loc, List<string> record)
        {
            loc.CountyCode = record[1];
            loc.StreetCode = record[2];
            loc.City = record[3];
            loc.Street = record[4];
            loc.PostalCode = record[8];
            loc.PostalDistrict = record[9];

        }
        private static void ReplaceChars(Location city)
        {
            //if (city.Street.Contains("å"))  
        }
        /// <summary>
        /// Returns current directory.
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentDirectory()
        {
            var rootPathChild = Directory.GetCurrentDirectory();
            var rootPathParent = Directory.GetParent($"{rootPathChild}");
            var rootPathFolder = Directory.GetParent($"{rootPathParent}");
            var rootPath = rootPathFolder.ToString();
            return rootPath;
        }

    }
}
