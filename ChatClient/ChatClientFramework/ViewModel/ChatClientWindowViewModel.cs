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
  
        private readonly object m_chatHistoryLockObject = new object();

        public string Name
        {
            get { return m_name; }
            set { if (changedName == false)
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
            BindingOperations.EnableCollectionSynchronization(UsersList, m_chatHistoryLockObject);//

            myTimer.Elapsed += new ElapsedEventHandler(myEvent);
            myTimer.Interval = 4000;
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



        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
