using System;
using System.Collections.Generic;
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
            Task.Factory.StartNew(() => Pulse());
            ReadRecordFromFile();
        }

        List<string> RecordSegments = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

        // This dictionary holds all record types and their segment positional values. Refactor
        private Dictionary<string, int[]> RecordTypeDict = new Dictionary<string, int[]>()
        {
            { "001", new int[]{ 3, 4, 4, 12, 4, 4, 4, 4, 12, 20, 40 } }, // AKTVEJ
            //{ "002", new int[]{ 3, 4, 4, 4, 2, 4, 12, 1, 12, 12, 34} }, // BOLIG
            { "003", new int[]{ 3, 4, 4, 4, 4, 1, 12, 34 } }, // BYNAVN
            { "004", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 20 }}, //POSTDIST
            //{ "005", new int[]{ 3, 4, 4, 2, 40, 12, 12 } }, // NOTATVEJ
            //{ "006", new int[]{ 3, 4, 4, 4, 4, 1, 12, 6, 30 } }, // BYFORNYDIST
            //{ "007", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 4, 30 } }, // DIVDIST
            //{ "008", new int[]{ 3, 4, 4, 4, 4, 1, 12, 1, 30 } }, // EVAKUERDIST
            { "009", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // KIRKEDIST
            //{ "010", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // SKOLEDIST
            //{ "011", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 30 } }, // BEFOLKDIST
            //{ "012", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // SOCIALDIST
            //{ "013", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 20 } }, // SOGNEDIST
            //{ "014", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // VALGDIST
            //{ "015", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 30 } }, // VARMEDIST
        };

        /// <summary>
        /// Read from .txt file
        /// </summary>
        private void ReadRecordFromFile()
        {
            // Clear database tables to make room for a new data extraction.
            DatabaseHandler.ClearTables();

            // Keep count of how many record have been decoded and sent to the database.
            int recordCount = 0;

            var rootPathChild = Directory.GetCurrentDirectory();
            var rootPathParent = Directory.GetParent($"{rootPathChild}");
            var rootPathFolder = Directory.GetParent($"{rootPathParent}");
            var rootPath = rootPathFolder.ToString();

            Location model = new Location();
            string currentLine = string.Empty;
            Stopwatch sw = new Stopwatch();
            List<Location> locationsList = new List<Location>();
            using (StreamReader sr = File.OpenText(rootPath + @"\full.txt"))
            {
                sw.Start();
                List<string> tempList = new List<string>();
                while ((currentLine = sr.ReadLine()) != null)
                {
                    string tempLine = currentLine.Substring(0, 3);
                    if (RecordTypeDict.Keys.Contains(currentLine.Substring(0, 3)))
                        tempList = SpliceRecord(currentLine, RecordTypeDict[tempLine]);

                    // Instantiate a new Location object if the current line is a record of type "001".
                    if (tempLine == "001")
                    {
                        // The first record doesn't have any data yet.
                        if (recordCount > 0)
                            locationsList.Add(model);
                        model = new Location();
                        //DatabaseHandler.AddData(model);   
                    }
                    recordCount++;

                    if (currentLine.Substring(0, 3) != "000")
                        BuildLocation(model, tempList);

                    // Keep count of handled records and total records
                    if (currentLine.Substring(0, 3) == "999")
                    {
                        Debug.WriteLine("Amount of locations added to database: " + locationsList.Count);
                        Debug.WriteLine("Total records: " + Int32.Parse(currentLine.Substring(4)) + "\n" + "Amount of handled records: " + (recordCount - 2));
                        Debug.WriteLine((Int32.Parse(currentLine.Substring(4))) == recordCount - 2);
                    }
                }
                DatabaseHandler.AddData(locationsList);
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
                case "000":
                case "002":
                case "005":
                case "013":
                case "999":
                    //Intet relevant
                    break;
                default:
                    loc.Distrikt = record[8];
                    break;


            }
        }
        private async void Pulse()
        {
            while (true)
            {
                //ReadRecordFromFile();
                await Task.Delay(10000);
            }

        }
    }
}
