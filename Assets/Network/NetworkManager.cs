using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using Google.Protobuf;


using MessageType = PlayerInfo.Types.MessageType;
using Direction = PlayerInfo.Types.Direction;

public class NetworkManager : MonoBehaviour
{
    public enum NetworkType
    {
        None, Server, Client
    }

    [SerializeField] private GameObject localPlayer;
    NetworkType networkType = NetworkType.None;
    private bool canUpdate = false;
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private Thread serverThread;
    private MemoryStream messageStream;
    private byte[] buffer = new byte[1024];
    SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
    SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
    private Queue<Action> Dispatcher = new Queue<Action>();
    void Awake()
    {

    }

    void Update()
    {
        while (Dispatcher.Count > 0) Dispatcher.Dequeue().Invoke();
    }
    private void OnDestroy()
    {
        messageStream?.Close();
        messageStream?.Dispose();
    }
    private void NetworkUpdate()
    {
        while (Dispatcher.Count > 0) Dispatcher.Dequeue().Invoke();
    }
    private void ListenClient(Socket client)
    {
        recvArgs.UserToken = client;
        byte[] buffer = new byte[1024];
        recvArgs.SetBuffer(buffer, 0, buffer.Length);
        bool willRaiseEvent = socket.ReceiveAsync(recvArgs);
        if (!willRaiseEvent)
        {
            ProcessReceive(recvArgs);
        }
    }

    public void SetupServer()
    {
        networkType = NetworkType.Server;
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, 25000));
        socket.Listen(0);
        //canUpdate = true;
        Debug.Log("Listening clients");
        acceptArgs.Completed += AcceptCompleted;
        AcceptConnection(acceptArgs);
    }

    private void AcceptConnection(SocketAsyncEventArgs args)
    {
        bool willRaiseEvent = socket.AcceptAsync(args);
        if (!willRaiseEvent)
        {
            AcceptCompleted(socket, args);
        }
    }

    private void AcceptCompleted(object sender, SocketAsyncEventArgs args)
    {
        if(args.SocketError == SocketError.Success)
        {
            ListenClient(args.AcceptSocket);
            Debug.Log("Client connected");
        }
        args.AcceptSocket = null;
        AcceptConnection(args);
    }

    public void ConnectAsClient()
    {
        Task task = socket.ConnectAsync("127.0.0.1", 25000).ContinueWith(task => {
            Debug.Log(socket.Connected ? "Connected" : "Connection faulted");
            SendMessageConnect();
        });
    }

    private void ProcessReceive(SocketAsyncEventArgs e)
    {
        Debug.Log("Process receive");
        if(e.BytesTransferred != 0 && e.SocketError == SocketError.Success)
        {
            int size = BitConverter.ToInt32(e.Buffer, 0);
            ProcessMessage((Socket)e.UserToken, PlayerInfo.Parser.ParseFrom(e.Buffer, 4, size));
            Debug.Log("Message size: " + size);
        }
        recvArgs = null;
        ListenClient((Socket)e.UserToken);
    }

    private void ProcessMessage(Socket client, PlayerInfo info)
    {
        switch (info.Type)
        {
            case MessageType.Connect:
                Debug.Log($"{client.RemoteEndPoint}: Connected");
                break;
        }
    }

    private void SendMessageConnect()
    {
        PlayerInfo info = new PlayerInfo
        {
            Type = MessageType.Connect
        };
        SocketAsyncEventArgs e = new SocketAsyncEventArgs();
        e.Completed += (o, e) => Debug.Log("Send message successfully");
        MemoryStream ms = new MemoryStream();
        ms.Write(BitConverter.GetBytes(info.CalculateSize()), 0, sizeof(int));
        info.WriteTo(ms);
        e.SetBuffer(ms.ToArray(), 0, (int) ms.Length);
        socket.SendAsync(e);
    }
}
