using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwitchApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<string> ChatList = new ObservableCollection<string>();

        TwitchClient client;

        public MainWindow()
        {
            Bot bot = new Bot();
            InitializeComponent();
            ChatBox.ItemsSource = ChatList;
        }

        private void ChatBox_DragEnter(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Search_Btn_Click(object sender, RoutedEventArgs e)
        {
            ConnectionCredentials credentials = new ConnectionCredentials("ToastedToastie", "v1vyufbf7ljw0bsgozva0zym3zzcov");

            string input = SearchBox.Text;


            client = new TwitchClient();
            client.Initialize(credentials, input);
            client.JoinRoom(input, null, false);

            client.OnConnected += Client_OnConnected;
            //client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            //client.OnNewSubscriber += Client_OnNewSubscriber;

            client.Connect();



        }
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatList.Add($"Connected to {e.AutoJoinChannel}\n");
            }
            ));
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            ChatList.Add($"Connected to {e.Channel.ToString()}'s chat room!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatList.Add($"{e.ChatMessage.Username}: {e.ChatMessage.Message}");
                xx.ScrollToBottom();
            }), DispatcherPriority.Background);
        }

        //private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        //{
        //    TestingBox.Text = ($"{e.Subscriber.SystemMessageParsed}");
        //}

        //private void Client_OnResubcriber(object sender, OnReSubscriberArgs e)
        //{
        //    TestingBox.Text = ($"{e.ReSubscriber.SystemMessageParsed}");
        //}
    }
}
