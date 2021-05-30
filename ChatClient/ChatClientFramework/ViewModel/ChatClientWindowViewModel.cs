using Google.Protobuf.WellKnownTypes;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using ChatClientConsole;

namespace ChatClientFramework
{
    public class ChatClientWindowViewModel : BindableBase
    {
        Boolean changedName = false;
        private readonly ChatServiceClient m_chatService = new ChatServiceClient();

        public ObservableCollection<string> ChatHistory { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> UsersList{ get; } = new ObservableCollection<string>();

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

            WriteCommand = new DelegateCommand<string>(WriteCommandExecute);

            StartReadingChatServer();
            StartReadingUsersServer();//
        }

        private void StartReadingChatServer()
        {
            var cts = new CancellationTokenSource();
            _ = m_chatService.ChatLogs()
                .ForEachAsync((x) => ChatHistory.Add($"{x.Time.ToDateTime().ToString("HH:mm:ss")} {x.Name}: {x.Content}"), cts.Token);

            App.Current.Exit += (_, __) => cts.Cancel();
        }

        private void StartReadingUsersServer()
        {
            var cts = new CancellationTokenSource();
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
    }
}
