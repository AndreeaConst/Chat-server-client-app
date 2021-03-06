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

        public ObservableCollection<Tuple<string, FontStyle, string, string>> ChatHistory { get; } = new ObservableCollection<Tuple<string, FontStyle, string, string>>();

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
                .ForEachAsync((x) => {
                    foreach (var newChat in FormatText(x.Time.ToDateTime().ToString("HH:mm:ss"), x.Name, x.Content))
                    ChatHistory.Add(new Tuple<string, FontStyle, string, string>(newChat.Item1, newChat.Item2, newChat.Item3, newChat.Item4));
                }, cts.Token) ;

            App.Current.Exit += (_, __) => cts.Cancel();
        }

        private void StartReadingUsersServer()
        {
            var cts = new CancellationTokenSource();
            UsersList.Clear();
            _ = m_chatService.Users()
                .ForEachAsync((x) => UsersList.Add($" {x.Name}"), cts.Token);

            App.Current.Exit += (_, __) => cts.Cancel();
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

        public ObservableCollection<Tuple<string, FontStyle, string, string>> FormatText(string time, string name, string text)
        {
            string text2 = text;
            Regex italic = new Regex("^.* ?_[a-zA-Z]+_.*$");
            Regex bold = new Regex("^.* \\*[a-zA-Z]+\\* .*$");
            Regex underline = new Regex("^.* `[a-zA-Z]+` .*$");
            Regex strikeout = new Regex("^.* ~[a-zA-Z]+~ .*$");

            ObservableCollection<Tuple<string, FontStyle, string, string>> messages = new ObservableCollection<Tuple<string, FontStyle, string, string>>();
            messages.Add(new Tuple<string, FontStyle, string, string>(time + " " + name + ": ", FontStyles.Normal, "", ""));
            if (italic.IsMatch(text) || bold.IsMatch(text) || underline.IsMatch(text) || strikeout.IsMatch(text))
            {
                string[] splitt = text2.Split();
                foreach (string g in splitt)
                {
                    if (g.StartsWith("_") && g.EndsWith("_"))
                    {
                        string s1 = g.Remove(0, 1);
                        string s2 = s1.Remove(s1.Length - 1);
                        messages.Add(new Tuple<string, FontStyle, string, string>(s2, FontStyles.Italic, "", ""));
                    }
                    else if (g.StartsWith("*") && g.EndsWith("*"))
                    {
                        string s1 = g.Remove(0, 1);
                        string s2 = s1.Remove(s1.Length - 1);
                        messages.Add(new Tuple<string, FontStyle, string, string>(s2,FontStyles.Normal, "", "Bold"));
                    }
                    else if (g.StartsWith("`") && g.EndsWith("`"))
                    {
                        string s1 = g.Remove(0, 1);
                        string s2 = s1.Remove(s1.Length - 1);
                        messages.Add(new Tuple<string, FontStyle, string, string>(s2, FontStyles.Normal, "Underline", ""));
                    }
                    else if (g.StartsWith("~") && g.EndsWith("~"))
                    {
                        string s1 = g.Remove(0, 1);
                        string s2 = s1.Remove(s1.Length - 1);
                        messages.Add(new Tuple<string, FontStyle, string, string>(s2, FontStyles.Normal, "Strikethrough", ""));
                    }
                    else
                    {
                        messages.Add(new Tuple<string, FontStyle, string, string>(g, FontStyles.Normal, "", ""));

                    }
           
                }
                return messages;
            }
            else messages.Add(new Tuple<string, FontStyle,string,string>(text2, FontStyles.Normal, "", ""));
            return messages;
        }
    }
}