using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems
{
    /// <summary>
    /// Handles record segmentation
    /// </summary>
    public class RecordHandler
    {
        public RecordHandler()
        {
            RecordTypeDict.Add("ATKVEJ", POSTDISTArr);

            // yikes
            recordList.Add(tempRecordType);
            recordList.Add(tempKOMKOD);
            recordList.Add(tempvejkod);
            recordList.Add(temptimestamp);
            recordList.Add(temptilkomkod);
            recordList.Add(temptilvejkod);
            recordList.Add(tempfrakomkod);
            recordList.Add(tempfravejkod);
            recordList.Add(temphaenstart);
            recordList.Add(tempvejadrnvn);
            recordList.Add(tempvejadrnvn);

            ReadRecordFromFile();
        }

        // yikes
        private string tempRecordType = string.Empty;
        private string tempKOMKOD = string.Empty;
        private string tempvejkod = string.Empty;
        private string temptimestamp = string.Empty;
        private string temptilkomkod = string.Empty;
        private string temptilvejkod = string.Empty;
        private string tempfrakomkod = string.Empty;
        private string tempfravejkod = string.Empty;
        private string temphaenstart = string.Empty;
        private string tempvejadrnvn = string.Empty;
        private string tempvejnvn = string.Empty;

        // Er ikke helt sikker på om det er den bedste måde at gøre det på, men det ser 10x bedre ud lol
        string[] RecordSegments = new string[11] {string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                                                  string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                                                  string.Empty};

        Dictionary<string, string> RecordData = new Dictionary<string, string>()
        {
            { "RECORDTYPE", string.Empty},
        };

        // Skal fjernes
        List<string> recordList = new List<string>();

        // Skal fjernes
        int[] POSTDISTArr = new int[9] { 3, 4, 4, 4, 4, 1, 12, 4, 20 };


        // This dictionary holds all record types and their segment positional values.
        private Dictionary<string, int[]> RecordTypeDict = new Dictionary<string, int[]>()
        {
            { "001", new int[]{ 3, 4, 4, 12, 4, 4, 4, 4, 12, 20, 40 } }, // AKTVEJ
            { "002", new int[]{ 3, 4, 4, 4, 2, 4, 12, 1, 12, 12, 34} }, // BOLIG
            { "003", new int[]{ 3, 4, 4, 4, 4, 1, 12, 34 } }, // BYNAVN
            { "004", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 20 }}, //POSTDIST
            { "005", new int[]{ 3, 4, 4, 2, 40, 12, 12 } }, // NOTATVEJ
            { "006", new int[]{ 3, 4, 4, 4, 4, 1, 12, 6, 30 } }, // BYFORNYDIST
            { "007", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 4, 30 } },
            { "008", new int[]{ 3, 4, 4, 4, 4, 1, 12, 1, 30 } },
            { "009", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "010", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "011", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 30 } },
            { "012", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "013", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 20 } },
            { "014", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "015", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 30 } },

            /*
            { "AKTVEJ", new int[]{ 3, 4, 4, 12, 4, 4, 4, 4, 12, 20, 40 } },
            { "BOLIG", new int[]{ 3, 4, 4, 4, 2, 4, 12, 1, 12, 12, 34} },
            { "BYNAVN", new int[]{ 3, 4, 4, 4, 4, 1, 12, 34 } },
            { "POSTDIST", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 20 }},
            { "NOTATVEJ", new int[]{ 3, 4, 4, 2, 40, 12, 12 } },
            { "BYFORNYDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 6, 30 } },
            { "DIVDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 4, 30 } },
            { "EVAKUERDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 1, 30 } },
            { "KIRKEDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "SKOLEDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "BEFOLKDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 30 } },
            { "SOCIALDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "SOGNEDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 20 } },
            { "VALGDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } },
            { "VARMEDIST", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 30 } },
            */
        };


        /// <summary>
        /// Read from .txt file
        /// </summary>
        private void ReadRecordFromFile()
        {
            var rootPathChild = Directory.GetCurrentDirectory();
            var rootPathParent = Directory.GetParent($"{rootPathChild}");
            var rootPathFolder = Directory.GetParent($"{rootPathParent}");
            var rootPath = rootPathFolder.ToString();
            string currentLine = string.Empty;
            using (StreamReader sr = File.OpenText(rootPath + @"\tempRecords.txt"))
            {
                while ((currentLine = sr.ReadLine()) != null)
                {
                    string tempLine = currentLine.Substring(0, 3);
                    SpliceRecord(currentLine, RecordTypeDict[tempLine]);
                }

                //while ((currentLine = sr.ReadLine()) != null)
                //{
                //    switch (currentLine.Substring(0, 3))
                //    {
                //        case "001":
                //            break;
                //        case "002":
                //            break;
                //        case "003":
                //            break;
                //        case "004":
                //            Debug.WriteLine(currentLine);
                //            SpliceRecord(currentLine, RecordTypeDict["POSTDIST"]);
                //            break;
                //        case "005":
                //            break;
                //        case "006":
                //            break;
                //        case "007":
                //            break;
                //        case "008":
                //            break;
                //        default:
                //            break;
                //    }
                //}

            }
        }

        /// <summary>
        /// Split a record into individual segments
        /// </summary>
        /// <param name="currentRecord"></param>
        /// <param name="recordType"></param>
        private void SpliceRecord(string currentRecord, int[] recordType)
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
            foreach (var item in RecordSegments)
            {
                Debug.WriteLine(item);
            }
            Debug.WriteLine("\n\n");
        }
    }
}
