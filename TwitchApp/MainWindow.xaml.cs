using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using System.Linq;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;

namespace TwitchApp
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        //DB Declare and Instantiation
        Model1Container db = new Model1Container();
        //Client Declare and Instantiation
        TwitchClient client = new TwitchClient();
        //Obersvable Declare and Instantiation
        public ObservableCollection<string> ChatList = new ObservableCollection<string>();

        //Event Handler for INotify
        public event PropertyChangedEventHandler PropertyChanged;

        //INotify
        private int _NoOfMessages;
        //INotify
        public int NoOfMessages
        {
            get
            {
                return _NoOfMessages;
            }
            set
            {
                _NoOfMessages = value;
                RaisePropertyChanged("NoOfMessages");

            }
        }

        //Window Intialisation
        public MainWindow()
        {
            InitializeComponent();
            ChatBox.ItemsSource = ChatList;
        }

        //RaiseProperty For INotify
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //Search for channel
        private void Search_Btn_Click(object sender, RoutedEventArgs e)
        {
            //Credentials to login with; for Authentication.
            ConnectionCredentials credentials = new ConnectionCredentials("ToastedToastie", "v1vyufbf7ljw0bsgozva0zym3zzcov");

            string input = SearchBox.Text;

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please enter a streamers alias");
            }
            else
            {
                //Initiate Client for login and join
                client.Initialize(credentials, input);
                client.JoinRoom(input, null, false);

                //Calling Methods
                client.OnConnected += Client_OnConnected;
                client.OnJoinedChannel += Client_OnJoinedChannel;
                client.OnMessageReceived += Client_OnMessageReceived;
                client.OnNewSubscriber += Client_OnNewSubscriber;
                client.OnReSubscriber += Client_OnResubcriber;

                SearchBox.Clear();

                client.Connect();
            }
        }

        //Connected to Channel Message
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                ChatList.Add($"Connecting to {e.AutoJoinChannel}\n");
            }
            ));
        }

        //Joining Channel Message
        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                ChatList.Add($"Connected to {e.Channel.ToString()}'s chat room!");
            }), DispatcherPriority.Background);
        }

        //Store message into Collection when recieved using event
        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                ChatList.Add($"{e.ChatMessage.Username}: {e.ChatMessage.Message}");

                //INotify
                NoOfMessages++;
                messagesNoTxt.Text = NoOfMessages.ToString();

                //Storing UserName and Message into database
                ChatLog tl = new ChatLog()
                {
                    Name = e.ChatMessage.Username,
                    Text = e.ChatMessage.Message
                };
                db.ChatLogs.Add(tl);
                db.SaveChanges();

                //Basically a way for autoscrolling as ScrollToBottom() Method doesn't work for me.
                ChatBox.SelectedIndex = ChatBox.Items.Count - 1;
                ChatBox.ScrollIntoView(ChatBox.SelectedItem);

            }), DispatcherPriority.Background);
        }

        //Subscription Message
        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                ChatList.Add($"{e.Subscriber.SystemMessageParsed}");

                //INotify
                NoOfMessages++;
                messagesNoTxt.Text = NoOfMessages.ToString();
            }), DispatcherPriority.Background);
        }

        //Resubscription Message
        private void Client_OnResubcriber(object sender, OnReSubscriberArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                ChatList.Add($"{e.ReSubscriber.SystemMessageParsed}");

                //INotify
                NoOfMessages++;
                messagesNoTxt.Text = NoOfMessages.ToString();
            }), DispatcherPriority.Background);
        }

        //Save Chat to JSON File
        private void JsonSave_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonConvert.SerializeObject(ChatList, Formatting.Indented);

            using (StreamWriter sw = new StreamWriter(@"..\..\ChatLog\ChatLog.json"))
            {
                sw.Write(json);
            }
        }

        //Button Click to Display ChatLogs in DataGrid
        private void RetrieveBtn_Click(object sender, RoutedEventArgs e)
        {
            var query = from m in db.ChatLogs
                        orderby m.Id descending
                        select new
                        {
                            Name = m.Name,
                            Message = m.Text
                        };

            dgChat.ItemsSource = query.ToList();
        }

    }
}
