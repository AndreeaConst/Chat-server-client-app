using ChatClientFramework;
using System.Windows;
using System.Windows.Input;

namespace GrpcWpfSample.Client.Wpf.View
{
    public partial class ChatClientWindow : Window
    {
        public ChatClientWindow()
        {
            InitializeComponent();
            DataContext = new ChatClientWindowViewModel();
        }

        private void BodyInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                (DataContext as ChatClientWindowViewModel).WriteCommand.Execute(BodyInput.Text);
                BodyInput.Text = "";
            }
        }

        private void BodyInput_Loaded(object sender, RoutedEventArgs e)
        {
            BodyInput.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            join.IsEnabled = false;
            NameInput.IsReadOnly = true;
        }
    }
}
