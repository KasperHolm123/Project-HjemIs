using Projekt_HjemIS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems
{
    public class DropzoneObserver
    {

        public DropzoneObserver()
        {
            ObserveDropzone();
        }

        // Watcher needs to be declared at the global scope to insure that it won't be disposed of.
        private FileSystemWatcher watcher = new FileSystemWatcher();

        /// <summary>
        /// Observes a folder for a new file.
        /// </summary>
        private void ObserveDropzone()
        {
            watcher.Path = $@"{GetCurrentDirectory()}\dropzone";
            watcher.Filter = "*.txt";
            watcher.Created += new FileSystemEventHandler(Watcher_Created);
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            RecordHandler.SaveRecords(RecordHandler.GetRecords());
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
