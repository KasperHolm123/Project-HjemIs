using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt_HjemIS.Models;

namespace Projekt_HjemIS.Systems
{
    /// <summary>
    /// Handles record segmentation
    /// </summary>
    public class RecordHandler
    {
        private List<Location> _locationsList = new List<Location>();

        public RecordHandler()
        {
            GetRecords();
            //ObserveDropZone();
        }

        // Holds a single record.
        private List<string> RecordSegments = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

        private int[] _postDist = new int[] { 3, 4, 4, 20, 20, 4, 4, 1, 4, 20 };

        /// <summary>
        /// Read from .txt file
        /// </summary>
        private List<Location> GetRecords()
        {
            // Keep count of how many record have been decoded and sent to the database.
            int recordCount = 0;

            Location currentLocation = new Location();
            Location prevLocation = new Location();
            using (StreamReader sr = File.OpenText(GetCurrentDirectory() + @"\dropzone\tempRecords.txt"))
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
                        case "000":
                            break;
                        case "999":
                            break;
                    }
                    prevLocation = currentLocation;
                    currentLocation = new Location();

                    // Keep count of handled records and total records
                    if (currentLine.Substring(0, 3) == "999")
                    {
                        Debug.WriteLine(
                            "Amount of locations added to database: " + _locationsList.Count +
                            "\nTotal records: " + Int32.Parse(currentLine.Substring(4)) +
                            "\nAmount of handled records: " + (recordCount - 1));
                    }
                    recordCount++;
                }
                return _locationsList;
            }
        }

        public void SaveRecords(List<Location> locations)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DataTable dt = ListToDataTableConverter.ToDataTable(locations);
            DatabaseHandler.AddBulkData(dt);
            Debug.WriteLine(sw.Elapsed);
            sw.Stop();
        }

        /// <summary>
        /// Split a record into individual segments
        /// </summary>
        /// <param name="currentRecord"></param>
        /// <param name="recordType"></param>
        private List<string> SpliceRecord(string currentRecord, int[] recordType)
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
        private void BuildLocation(Location loc, List<string> record)
        {
            loc.CountyCode = record[1];
            loc.StreetCode = record[2];
            loc.City = record[3];
            loc.Street = record[4];
            loc.PostalCode = record[8];
            loc.PostalDistrict = record[9];

        }

        // Watcher needs to be declared at the global scope to insure that it won't be disposed of.
        FileSystemWatcher watcher = new FileSystemWatcher();

        /// <summary>
        /// Observes a folder for a new file.
        /// </summary>
        private void ObserveDropZone()
        {
            watcher.Path = $@"{GetCurrentDirectory()}\dropzone";
            watcher.Filter = "*.txt";
            watcher.Created += new FileSystemEventHandler(Watcher_Created);
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            SaveRecords(_locationsList);
        }

        /// <summary>
        /// Returns current directory.
        /// </summary>
        /// <returns></returns>
        private string GetCurrentDirectory()
        {
            var rootPathChild = Directory.GetCurrentDirectory();
            var rootPathParent = Directory.GetParent($"{rootPathChild}");
            var rootPathFolder = Directory.GetParent($"{rootPathParent}");
            var rootPath = rootPathFolder.ToString();
            return rootPath;
        }

    }
}
