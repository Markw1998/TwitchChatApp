using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

//Bot Class For Testing
class Bot
{
    TwitchClient client;

    public Bot()
    {
        ConnectionCredentials credentials = new ConnectionCredentials("ToastedToastie", "v1vyufbf7ljw0bsgozva0zym3zzcov");

        Console.WriteLine("Enter streamer name: ");
        string input = Console.ReadLine();

        client = new TwitchClient();
        client.Initialize(credentials, input);
        client.JoinRoom(input, null, false);

        //client.OnLog += Client_OnLog;
        client.OnConnected += Client_OnConnected;
        client.OnJoinedChannel += Client_OnJoinedChannel;
        client.OnMessageReceived += Client_OnMessageReceived;
        client.OnNewSubscriber += Client_OnNewSubscriber;

        client.Connect();

    }

    private void Client_OnLog(object sender, OnLogArgs e)
    {
        Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        Console.WriteLine($"Connected to {e.AutoJoinChannel}\n");
    }

    private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Console.WriteLine($"Connected to {e.Channel.ToString()}'s chat room!");
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        if (e.ChatMessage.Message.Contains("@"))
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{e.ChatMessage.DisplayName}: {e.ChatMessage.Message}");
        }
    }

    private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
    {
        Console.WriteLine($"{e.Subscriber.SystemMessageParsed} Just Subscribed!");
    }

    private void Client_OnResubcriber(object sender, OnReSubscriberArgs e)
    {
        Console.WriteLine($"{e.ReSubscriber.SystemMessageParsed}");
    }

}