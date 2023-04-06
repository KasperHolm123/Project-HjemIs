using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.ViewModels
{
    public class EmulatorViewModel : BaseViewModel
    {
        #region Fields

        private List<string> _messages { get; set; }
        public List<string> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        private readonly string _rootPath = $@"\tempMessages\InternalMessages.txt";

        #endregion

        public EmulatorViewModel()
        {
            Messages = new List<string>();

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
                    if (currentLine == "")
                        currentMessage += "\n";
                    if (currentLine != "")
                    {
                        string sub = currentLine.Substring(0, 3);
                        switch (sub)
                        {
                            case @"00/":
                            case @"02/":
                            case @"03/":
                                currentMessage += currentLine.Substring(3) + "\n";
                                break;
                            case @"04/":
                                Messages.Add(currentMessage);
                                currentMessage = string.Empty;
                                break;
                            default:
                                currentMessage += currentLine + "\n";
                                break;
                        }
                    }
                }
            }
        }
    }
}
