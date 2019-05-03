using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace TwitchApp
{
    public partial class MainWindow : Window
    {
        Model1Container db = new Model1Container();
        public ObservableCollection<string> ChatList = new ObservableCollection<string>();
        TwitchClient client;

        public MainWindow()
        {
            Bot bot = new Bot();
            InitializeComponent();
            ChatBox.ItemsSource = ChatList;
        }

        //Search for channel
        private void Search_Btn_Click(object sender, RoutedEventArgs e)
        {
            //Credentials to login with for Authentication
            ConnectionCredentials credentials = new ConnectionCredentials("ToastedToastie", "v1vyufbf7ljw0bsgozva0zym3zzcov");

            string input = SearchBox.Text;

            //Initiate Client for login and join
            client = new TwitchClient();
            client.Initialize(credentials, input);
            client.JoinRoom(input, null, false);

            //Calling Methods
            client.OnConnected += Client_OnConnected;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;

            client.Connect();
        }

        //Connected to Channel Message
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatList.Add($"Connected to {e.AutoJoinChannel}\n");
            }
            ));
        }

        //Joining Channel Message
        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatList.Add($"Connected to {e.Channel.ToString()}'s chat room!");
            }), DispatcherPriority.Background);
        }

        //Store message into Collection when recieved using event
        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatList.Add($"{e.ChatMessage.Username}: {e.ChatMessage.Message}");
                xx.ScrollToBottom();

                //Storing UserName and Message into database
                ChatLog tl = new ChatLog()
                {
                    Name = e.ChatMessage.Username,
                    Text = e.ChatMessage.Message 
                };
                db.ChatLogs.Add(tl);
                db.SaveChanges();


            }), DispatcherPriority.Background);
        }

        //Subscription Message
        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatList.Add($"{e.Subscriber.SystemMessageParsed}");
        }), DispatcherPriority.Background);
        }

        //Resubscription Message
        private void Client_OnResubcriber(object sender, OnReSubscriberArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatList.Add($"{e.ReSubscriber.SystemMessageParsed}");
    }), DispatcherPriority.Background);
        }

        private void JsonSave_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(ChatList, Formatting.Indented);

            using (StreamWriter sw = new StreamWriter(@"..\..\ChatLog\ChatLog.json"))
            {
                sw.Write(json);
            }
        }
    }
}
