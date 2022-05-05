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
            Pulse();
            GetRecords();
        }

        // Holds a single record.
        private List<string> RecordSegments = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

        // This dictionary holds all record types and their segment positional values.
        private Dictionary<string, int[]> RecordTypeDict = new Dictionary<string, int[]>()
        {
            { "001", new int[]{ 3, 4, 4, 12, 4, 4, 4, 4, 12, 20, 40 } }, // AKTVEJ
            { "003", new int[]{ 3, 4, 4, 4, 4, 1, 12, 34 } }, // BYNAVN
            { "004", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 20 }}, //POSTDIST
            { "009", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // KIRKEDIST
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
            using (StreamReader sr = File.OpenText(GetCurrentDirectory() + @"\full.txt"))
            {
                sw.Start();

                // Read each line from current file.
                while ((currentLine = sr.ReadLine()) != null)
                {
                    string currentLineRecordType = currentLine.Substring(0, 3);

                    // Instantiate a new Location object if the current line is a record of type "001".
                    if (currentLineRecordType == "001")
                    {
                        // The first record doesn't have any data yet.
                        if (recordCount > 1)
                            locationsList.Add(currentLocation);
                        currentLocation = new Location();
                    }

                    if (RecordTypeDict.Keys.Contains(currentLine.Substring(0, 3)))
                        BuildLocation(currentLocation, SpliceRecord(currentLine, RecordTypeDict[currentLineRecordType]));

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
                ListToDataTableConverter converter = new ListToDataTableConverter();

                DataTable dt = converter.ToDataTable(locationsList);

                DatabaseHandler.AddBulkData(dt); // DatabaseHandler mangler refactoring.

                //DatabaseHandler.AddData(locationsList);

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
            switch (record[0])
            {
                case "001":
                    loc.Kommunekode = record[1]; //kommunekode
                    loc.Vejkode = record[2]; //vejkode
                    loc.VejNavn = record[10]; // 9 eller 10 er vejnavn
                    break;
                case "003":
                    loc.Bynavn = record[7];
                    break;
                case "004":
                    loc.PostNr = record[7];
                    loc.Postdistrikt = record[8];
                    break;
                default:
                    break;
            }
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

        private async void Pulse()
        {
            while (true)
            {
                //await Task.Run(()=>GetRecords()); //GetRecords() delegated to separate thread, await-es for ikke at blokere calling-thread
                await Task.Delay(10000);
            }

        }
    }
}
