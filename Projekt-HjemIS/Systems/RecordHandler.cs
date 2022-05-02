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
            ReadRecordFromFile();
        }

        // måske er denne måde bedre, men så kræver det et dictionary
        Dictionary<string, string> RecordData = new Dictionary<string, string>()
        {
            { "RECORDTYPE", string.Empty},
        };

        // This dictionary holds all record types and their segment positional values. Refactor
        private Dictionary<string, int[]> RecordTypeDict = new Dictionary<string, int[]>()
        {
            // Husk at ændre app.config til jeres egen PC
            { "001", new int[] { 3, 4, 4 } }, // AKTVEJ
            { "002", new int[] { 3, 4, 4 } }, // BOLIG
            { "003", new int[] { 3, 4, 4 } }, // BYNAVN
            { "004", new int[] { 3, 4, 4 } }, //POSTDIST
            { "005", new int[] { 3, 4, 4 } }, // NOTATVEJ
            { "006", new int[] { 3, 4, 4 } }, // BYFORNYDIST
            { "007", new int[] { 3, 4, 4 } }, // DIVDIST
            { "008", new int[] { 3, 4, 4 } }, // EVAKUERDIST
            { "009", new int[] { 3, 4, 4 } }, // KIRKEDIST
            { "010", new int[] { 3, 4, 4 } }, // SKOLEDIST
            { "011", new int[] { 3, 4, 4 } }, // BEFOLKDIST
            { "012", new int[] { 3, 4, 4 } }, // SOCIALDIST
            { "013", new int[] { 3, 4, 4 } }, // SOGNEDIST
            { "014", new int[] { 3, 4, 4 } }, // VALGDIST
            { "015", new int[] { 3, 4, 4 } }, // VARMEDIST

            /*
            { "001", new int[]{ 3, 4, 4, 12, 4, 4, 4, 4, 12, 20, 40 } }, // AKTVEJ
            { "002", new int[]{ 3, 4, 4, 4, 2, 4, 12, 1, 12, 12, 34} }, // BOLIG
            { "003", new int[]{ 3, 4, 4, 4, 4, 1, 12, 34 } }, // BYNAVN
            { "004", new int[] { 3, 4, 4, 4, 4, 1, 12, 4, 20 }}, //POSTDIST
            { "005", new int[]{ 3, 4, 4, 2, 40, 12, 12 } }, // NOTATVEJ
            { "006", new int[]{ 3, 4, 4, 4, 4, 1, 12, 6, 30 } }, // BYFORNYDIST
            { "007", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 4, 30 } }, // DIVDIST
            { "008", new int[]{ 3, 4, 4, 4, 4, 1, 12, 1, 30 } }, // EVAKUERDIST
            { "009", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // KIRKEDIST
            { "010", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // SKOLEDIST
            { "011", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 30 } }, // BEFOLKDIST
            { "012", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // SOCIALDIST
            { "013", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 20 } }, // SOGNEDIST
            { "014", new int[]{ 3, 4, 4, 4, 4, 1, 12, 2, 30 } }, // VALGDIST
            { "015", new int[]{ 3, 4, 4, 4, 4, 1, 12, 4, 30 } }, // VARMEDIST
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
                    string currentLineRecordType = currentLine.Substring(0, 3);
                    DatabaseHandler.AddData(SpliceRecord(currentLine, RecordTypeDict[currentLineRecordType]));
                }
            }
        }

        /// <summary>
        /// Split a record into individual segments
        /// </summary>
        /// <param name="currentRecord"></param>
        /// <param name="recordType"></param>
        private string[] SpliceRecord(string currentRecord, int[] recordType)
        {
            string[] CurrentRecordSegments = new string[3] { string.Empty, string.Empty, string.Empty };
            int currentSegment = 0;
            
            for (int i = 0; i < recordType.Length; i++)
            {
                if (currentRecord != null)
                {
                    CurrentRecordSegments[i] = currentRecord.Substring(currentSegment, recordType[i]);
                    currentSegment += recordType[i];
                }
            }

            foreach (var item in CurrentRecordSegments)
            {
                Debug.WriteLine(item);
            }
            Debug.WriteLine("\n\n");

            return CurrentRecordSegments;
        }
    }
}
