using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems
{
    public enum RecordType
    {
        AKTVEJ = 001,
        BOLIG = 002,
        BYNAVN = 003,
        POSTVEJ = 004,
        NOTATVEJ = 005,
        BYFORNYDIST = 006,
    }

    public class RecordHandler
    {
        private Dictionary<string, int[]> RecordTypeDict = new Dictionary<string, int[]>();

        public RecordHandler()
        {
            RecordTypeDict.Add("ATKVEJ", AKTVEJArr);
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


        private int RECORDTYPE = 3;
        private int KOMKOD = 4;
        private int VEJKOD = 4;
        private int TIMESTAMP = 12;
        private int TILKOMKOD = 4;
        private int TILVEJKOD = 4;
        private int FRAVEJKOD = 4;

        private int currentCol = 0;

        private string tempRecord = "001001000701991092312000000000000000000190001010000Norge               Norge                                   ";

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

        
        
        List<string> recordList = new List<string>();

        int[] AKTVEJArr = new int[11] { 3, 4, 4, 12, 4, 4, 4, 4, 12, 20, 40};

        private void ReadRecordFromFile()
        {
            string rootPath = Directory.GetCurrentDirectory();
            using (StreamReader sr = File.OpenText(rootPath + @"\tempRecords.txt"))
            {
                while (sr.ReadLine() != null)
                {
                    switch (sr.ReadLine().Substring(0, 3))
                    {
                        case "001":
                            SpliceRecord(AKTVEJArr);
                            break;
                        case "002":
                            break;
                        case "003":
                            break;
                        case "004":
                            break;
                        case "005":
                            break;
                        case "006":
                            break;
                        case "007":
                            break;
                        case "008":
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        public void SplitRecord(RecordType recordType)
        {
            int colLength = 111;

            switch (recordType)
            {
                case RecordType.AKTVEJ:
                    Debug.WriteLine(tempRecord);
                    SpliceRecord(AKTVEJArr);
                    foreach (var item in recordList)
                    {
                        Debug.WriteLine(item);
                    }
                    break;
                case RecordType.BOLIG:
                    break;
                case RecordType.BYNAVN:
                    break;
                case RecordType.POSTVEJ:
                    break;
                case RecordType.NOTATVEJ:
                    break;
                case RecordType.BYFORNYDIST:
                    break;
                default:
                    break;
            }
        }

        private void SpliceRecord(int[] recordType)
        {
            for (int i = 0; i < 11; i++)
            {
                recordList[i] = tempRecord.Substring(currentCol, recordType[i]);
                currentCol += recordType[i];
            }
        }
    }
}
