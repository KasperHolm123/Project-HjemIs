using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt_HjemIS.Views
{
    /// <summary>
    /// Interaction logic for EmulatorView.xaml
    /// </summary>
    public partial class EmulatorView : UserControl
    {
        public List<string> Messages { get; set; }
        private string _rootPath = $@"\tempMessages\InternalMessages.txt";

        public EmulatorView()
        {
            InitializeComponent();

            // Setup collections
            Messages = new List<string>();

            // Execute methods
            ReadMessages();
        }

        private void ReadMessages()
        {
            using (StreamReader sr = new StreamReader($@"{GetCurrentDirectory()}{_rootPath}"))
            {
                string currentLine = string.Empty;
                string currentMessage = string.Empty;

                while ((currentLine = sr.ReadLine()) != null)
                {
                    string sub = currentLine.Substring(0, 3);
                    switch (sub)
                    {
                        case @"04/":
                            mainListBox.Items.Add(currentMessage);
                            currentMessage = string.Empty;
                            break;
                        default:
                            currentMessage += currentLine.Substring(3) + "\n";
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns current directory.
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentDirectory()
        {
            var rootPathChild = Directory.GetCurrentDirectory();
            var rootPathParent = Directory.GetParent($"{rootPathChild}");
            var rootPathFolder = Directory.GetParent($"{rootPathParent}");
            var rootPath = rootPathFolder.ToString();
            return rootPath;
        }
    }
}
