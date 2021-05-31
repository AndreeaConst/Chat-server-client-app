using Google.Protobuf.WellKnownTypes;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using ChatClientConsole;
using System.Windows.Input;
using System.Timers;
using System.Windows;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace ChatClientFramework
{
    public class ChatClientWindowViewModel : BindableBase
    {
        Boolean changedName = false;
        private System.Timers.Timer myTimer = new System.Timers.Timer();
        private readonly ChatServiceClient m_chatService = new ChatServiceClient();

        public ObservableCollection<string> ChatHistory { get; } = new ObservableCollection<string>();

        // public ObservableCollection<string> UsersList{ get; } = new ObservableCollection<string>();
        public ObservableCollection<string> usersList = new ObservableCollection<string>();
        public ObservableCollection<string> UsersList
        {
            get { return usersList; }
            set
            {
                usersList = value;
                NotifyPropertyChanged("UsersList");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly object m_chatHistoryLockObject = new object();

        public string Name
        {
            get { return m_name; }
            set
            {
                if (changedName == false)
                {
                    SetProperty(ref m_name, value);
                    changedName = true;
                }
            }
        }
        private string m_name = "Enter your nickname";

        public DelegateCommand<string> WriteCommand { get; }

        public ChatClientWindowViewModel()
        {
            Disable = true;
            BindingOperations.EnableCollectionSynchronization(ChatHistory, m_chatHistoryLockObject);
            BindingOperations.EnableCollectionSynchronization(UsersList, m_chatHistoryLockObject);


            myTimer.Elapsed += new ElapsedEventHandler(myEvent);
            myTimer.Interval = 5000;
            myTimer.Enabled = false;

            WriteCommand = new DelegateCommand<string>(WriteCommandExecute);
            ClickCommand = new DelegateCommand(OnClick);

        }
        private void myEvent(object source, ElapsedEventArgs e)
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                StartReadingUsersServer();
            });
        }

        public ICommand ClickCommand
        {
            get;
            private set;

        }
        public void OnClick()
        {
            StartReadingChatServer();
            StartReadingUsersServer();
            myTimer.Enabled = true;
        }

        private void StartReadingChatServer()
        {
            var cts = new CancellationTokenSource();
            _ = m_chatService.ChatLogs(new Username { Name = Name, })
                .ForEachAsync((x) => ChatHistory.Add($"{x.Time.ToDateTime().ToString("HH:mm:ss")} {x.Name}: {x.Content}"), cts.Token);

            App.Current.Exit += (_, __) => cts.Cancel();
        }

        private void StartReadingUsersServer()
        {
            var cts = new CancellationTokenSource();
            UsersList.Clear();
            _ = m_chatService.Users()
                .ForEachAsync((x) => UsersList.Add($" {x.Name}"), cts.Token);

            App.Current.Exit += (_, __) => cts.Cancel();
            Disable = false;
        }

        private async void WriteCommandExecute(string content)
        {
            await m_chatService.Write(new ChatLog
            {
                Name = m_name,
                Content = content,
                Time = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
            });


        }

        private bool disable;
        public bool Disable
        {
            get
            {
                return disable;
            }
            set
            {
                disable = value;
                NotifyPropertyChanged("Disable");
            }
        }

        public Dictionary<string, FontStyle> FormatText(string time, string name, string text)
        {
            Regex regex = new Regex("^.* ?_[a-zA-Z]+_.*$");
            string text2 = text;
            Dictionary<string, FontStyle> messages = new Dictionary<string, FontStyle>();
            messages.Add(time, FontStyles.Normal);
            messages.Add(name, FontStyles.Normal);
            if (regex.IsMatch(text))
            {
                string[] splitt = text2.Split();
                System.Text.StringBuilder n = new System.Text.StringBuilder();
                System.Text.StringBuilder numbers = new System.Text.StringBuilder();
                foreach (string g in splitt)
                {
                    if (g.StartsWith("_") && g.EndsWith("_"))
                    {
                        string s1 = g.Remove(0, 1);
                        string s2 = s1.Remove(s1.Length - 1);
                        FontStyle f = FontStyles.Italic;
                        messages.Add(s2, f);
                    }
                    else
                    {
                        messages.Add(g, FontStyles.Normal);
                        messages.Add(" ", FontStyles.Normal);
                    }

                }
                return messages;
            }
            else messages.Add(text2, FontStyles.Normal);
            return messages;
        }
    }
}