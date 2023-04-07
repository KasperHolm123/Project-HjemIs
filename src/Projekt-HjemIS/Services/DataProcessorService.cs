using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Services
{
    public class DataProcessorService
    {
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

        public void ProcessData()
        {
            var path = "../../dropzone/sample_data.txt";

            var timer = new Stopwatch();
            timer.Start();

            using (var reader = new StreamReader(path))
            {
                var currentLine = "";


                while ((currentLine = reader.ReadLine()) != null)
                {
                    var recordType = currentLine.Substring(0, 3);

                    var index = 0;
                    
                    foreach (var sectionLength in _recordTypes[recordType])
                    {
                        Debug.Write(currentLine.Substring(index, sectionLength) + " ");
                        index += sectionLength;
                    }

                    Debug.WriteLine("");
                }
            }

            timer.Stop();
            Debug.WriteLine(timer.Elapsed);
        }
    }
}
