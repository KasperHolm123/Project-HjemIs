using Projekt_HjemIS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt_HjemIS.Systems
{
    public class DropzoneObserver
    {

        public DropzoneObserver()
        {

        }

        // Watcher needs to be declared at the global scope to insure that it won't be disposed of.
        private FileSystemWatcher watcher = new FileSystemWatcher();

        /// <summary>
        /// Observes a folder for a new file.
        /// </summary>
        public void ObserveDropzone()
        {
            watcher.Path = $@"{GetCurrentDirectory()}\dropzone";
            watcher.Filter = "*.txt";
            watcher.Created += new FileSystemEventHandler(Watcher_Created);
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private async void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string result = await PromptRecordHandling();
            MessageBox.Show($"{result}");
        }

        private async Task<string> PromptRecordHandling()
        {
            string result = string.Empty;
            MessageBoxResult msgPrompt = MessageBox.Show("Nyt dataudtræk opdaget. Start behandling?", "Record handling", MessageBoxButton.YesNo);
            switch (msgPrompt)
            {
                case MessageBoxResult.Yes:
                    result = await RecordHandler.SaveRecords(RecordHandler.GetRecords());
                    break;
                default:
                    break;
            }
            return result;
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
