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

        public RecordHandler()
        {
            GetRecords();
            //ObserveDropZone();
        }

        // Holds a single record.
        private List<string> RecordSegments = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

        // This dictionary holds all record types and their segment positional values.
        private Dictionary<string, int[]> RecordTypeDict = new Dictionary<string, int[]>()
        {
            { "001", new int[] { 3, 4, 4, 20, 20, 4, 4, 1, 4, 20 } } // POSTDIST

            //{ "001", new int[]{ 3, 4, 4, 12, 4, 4, 4, 4, 12, 20, 40 } }, // AKTVEJ
            //{ "003", new int[]{ 3, 4, 4, 4, 4, 1, 12, 34 } }, // BYNAVN
            //{ "004", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 20 }}, //POSTDIST
            //{ "009", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // KIRKEDIST
        };

        /// <summary>
        /// Read from .txt file
        /// </summary>
        private void GetRecords()
        {
            Stopwatch sw = new Stopwatch();


            // Keep count of how many record have been decoded and sent to the database.
            int recordCount = 0;

            Location currentLocation = new Location();

            string currentLine = string.Empty;
            List<Location> locationsList = new List<Location>();
            using (StreamReader sr = File.OpenText(GetCurrentDirectory() + @"\dropzone\*.txt"))
            {
                sw.Start();
                // Read each line from current file.
                while ((currentLine = sr.ReadLine()) != null)
                {
                    BuildLocation(currentLocation, SpliceRecord(currentLine, RecordTypeDict["001"]));
                    locationsList.Add(currentLocation);
                    currentLocation = new Location();

                    // Keep count of handled records and total records
                    if (currentLine.Substring(0, 3) == "999")
                    {
                        Debug.WriteLine(
                            "Amount of locations added to database: " + locationsList.Count +
                            "\nTotal records: " + Int32.Parse(currentLine.Substring(4)) +
                            "\nAmount of handled records: " + (recordCount - 2));
                    }
                    recordCount++;
                }
                DataTable dt = ListToDataTableConverter.ToDataTable(locationsList);

                DatabaseHandler.AddBulkData(dt);

                Debug.WriteLine(sw.Elapsed);
                sw.Stop();
            }
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
            Debug.WriteLine($"new file: {e.FullPath}");
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
