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
using static PlayerInfo.Types;
using UnityEngine.Events;
using System.Linq;

public class NetworkEvent
{
    private Action _action;
    private string _actionName;
    public string ActionName
    {
        get => _actionName;
    }
    public void CallEvent() => _action.Invoke();
    public NetworkEvent(string name, Action action)
    {
        _action = action;
        _actionName = name;
    }
}

public class NetworkManager : MonoBehaviour
{
    public enum NetworkType
    {
        None, Server, Client
    }
    private SceneManager sceneManager;
    private LanguageManager languageManager;
    [SerializeField] private PlayerController localPlayer;
    public NetworkType networkType = NetworkType.None;
    private Socket m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public Socket Socket { get => m_socket; private set => m_socket = value; }
    private MemoryStream messageStream;
    SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
    private static Queue<NetworkEvent> UpdateDispatcher = new Queue<NetworkEvent>();
    private static Queue<NetworkEvent> FixedUpdateDispatcher = new Queue<NetworkEvent>();
    private List<PlayerController> players = new List<PlayerController>();
    private Dictionary<Socket, PlayerController> playersSocket = new Dictionary<Socket, PlayerController>();
    private int serverPort = 0;
    public static readonly object lockObj = new object();

    private void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        foreach (var arg in args)
        {
            if (arg.Contains("-port="))
            {
                serverPort = int.Parse(arg.Split('=')[1]);
                Debug.Log("Server Port from args: " + serverPort);
                break;
            }
        }
        Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
    }

    private void Start()
    {
        sceneManager = GetComponent<SceneManager>();
        languageManager = GetComponent<LanguageManager>();
#if UNITY_SERVER
        SetupServer();
        sceneManager.SetScene("city");
        sceneManager.ToggleVolume();
        languageManager.LoadTextFlags(sceneManager.GetCurrentScene());
        localPlayer.gameObject.SetActive(false);
#endif
    }
    void Update()
    {
        while (UpdateDispatcher.Count > 0)
        {
            var evnt = UpdateDispatcher.Dequeue();
            Debug.Log(evnt.ActionName);
            evnt.CallEvent();
        }
        RefreshPlayerLayers();
    }
    private void FixedUpdate()
    { 
        while (FixedUpdateDispatcher.Count > 0)
        {
            var evnt = FixedUpdateDispatcher.Dequeue();
            if (evnt == null)
                Debug.Log("Event is null");
            Debug.Log(evnt.ActionName);
            evnt.CallEvent();
        }
    }
    private void OnDestroy()
    {
        messageStream?.Close();
        messageStream?.Dispose();
        if (Socket.Connected)
            Socket.Shutdown(SocketShutdown.Both);
        Socket?.Close();
        Socket?.Dispose();
    }
    public void SetupServer(TMPro.TMP_InputField input)
    {
        int port;
        if (!int.TryParse(input.text, out port) || port > ushort.MaxValue)
        {
            input.text = string.Empty;
            return;
        }
        SetupServer(port);
    }
    public void SetupServer(int? port = null)
    {
        networkType = NetworkType.Server;
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#if UNITY_SERVER
        socket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
#else
        if (!port.HasValue || port > ushort.MaxValue) return;
        Socket.Bind(new IPEndPoint(IPAddress.Any, port.Value));
#endif
        Socket.Listen(0);
        localPlayer.playerGuid = Guid.NewGuid();
        //canUpdate = true;
        sceneManager.SetScene("city");
        languageManager.LoadTextFlags(sceneManager.GetCurrentScene());
        Debug.Log("Listening clients");
        acceptArgs.Completed += AcceptCompleted;
        AcceptConnection(acceptArgs);
    }
    private void AcceptConnection(SocketAsyncEventArgs args)
    {
        Socket.AcceptAsync(args);
    }
    private void AcceptCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            ListenClient(args.AcceptSocket);
            Debug.Log("Client connected");
        }
        args.AcceptSocket = null;
        AcceptConnection(args);
    }
    private async void ListenClient(Socket client)
    {
        await Task.Run(() =>
        {   
            while(true)
            {
                ReceiveMessage(client);
            }
        });
    }
    private async void ReceiveMessage(Socket client)
    {
        byte[] buffer = new byte[4];
        byte[] protoBuffer = new byte[1024];
        ProtoPacket packet;
        int size = 0;
        int protoMsgSize = 0;
        try
        {
            size = client.Receive(buffer);
            if (size < 4)
            {
                AddEvent(nameof(ListenClient), () => RemovePlayer(client));
                return;
            }
            protoMsgSize = BitConverter.ToInt32(buffer, 0);
            packet = new ProtoPacket();
            while (protoMsgSize > 0)
            {
                int rcvSize = client.Receive(protoBuffer);
                protoMsgSize -= rcvSize;
                if (protoMsgSize < protoBuffer.Length)
                {
                    packet.AddBytes(protoBuffer, rcvSize);
                    break;
                }
                packet.AddBytes(protoBuffer);
            }
            await ProcessMessageAsync(client, packet);
        }
        catch (SocketException e)
        {
            Debug.LogError(e.Message);
        }
    }
    private async Task ProcessMessageAsync(Socket client, ProtoPacket packet) => await Task.Run(() => ProcessMessage(client, packet));
    private void ProcessMessage(Socket client, ProtoPacket packet)
    {
        #region LegacyProcess
        PlayerInfo info = packet.IsDefault ? new PlayerInfo() : PlayerInfo.Parser.ParseFrom(packet.GetBytes());
        Vector2 startPos = new Vector2(-20.7f, 1.37f);
        switch (info.Type)
        {
            //SERVER SIDE MESSAGES
            case MessageType.Connect:
                Debug.Log($"{client.RemoteEndPoint}: Connected");
                Guid playerGuid = Guid.NewGuid();
                AddEvent(info.Type.ToString(), () => CreateNewPlayer(startPos, playerGuid, client));
                PlayerInfo infoReq = new PlayerInfo { Type = MessageType.AcceptConnect, Position = new vec2f { X = startPos.x, Y = startPos.y }, Guid = playerGuid.ToString("N") };
                SendMessageTo(client, infoReq);
                SendConnectedPlayerToOthers(client, infoReq);
                break;

            case MessageType.InfoRequest:
                AddEvent(info.Type.ToString(), () => SendAllPlayersInfoTo(client));
                break;

            case MessageType.Move:
                //playersSocket[client].RememberOldPos();
                AddFixedEvent(info.Type.ToString(), () =>
                {
                    PlayerController player = playersSocket[client];
                    Vector2 direction = new Vector2(info.Direction.X, info.Direction.Y);
                    player.MoveLocal(direction);
                    player.RefreshAnimation(direction);
                    info.Type = MessageType.AcceptMove;
                    info.Position = new vec2f { X = player.transform.position.x, Y = player.transform.position.y };
                    SendMessageTo(client, info);
                    PlayerInfo info2 = new PlayerInfo(info);
                    info2.Type = MessageType.PlayerMoves;
                    SendPlayerMovementToAll(client, info2);
                });
                break;

            //CLIENT SIDE MESSAGES
            case MessageType.AcceptConnect:
                AddEvent(info.Type.ToString(), () => LoadNetworkScene("city", info));
                SendMessageTo(Socket, new PlayerInfo { Type = MessageType.InfoRequest });
                break;
            case MessageType.NewPlayerInfo:
                AddEvent(info.Type.ToString(), () => CreateNewPlayer(new Vector2(info.Position.X, info.Position.Y), Guid.ParseExact(info.Guid, "N")));
                break;

            case MessageType.AcceptMove:
                AddFixedEvent(info.Type.ToString(), () =>
                {
                    Vector2 direction = new Vector2(info.Direction.X, info.Direction.Y);
                    Vector2 position = new Vector2(info.Position.X, info.Position.Y);
                    localPlayer.MoveLocal(direction);
                    localPlayer.RefreshAnimation(direction);
                });
                break;
            case MessageType.NewPlayerConnected:
                AddEvent(info.Type.ToString(), () => CreateNewPlayer(new Vector2(info.Position.X, info.Position.Y), Guid.ParseExact(info.Guid, "N")));
                break;
            case MessageType.PlayerMoves:
                MovePlayer(info);
                break;
            case MessageType.PlayerDisconnected:
                AddEvent(info.Type.ToString(), () => RemovePlayer(Guid.ParseExact(info.Guid, "N")));
                break;
        }
        #endregion
    }

    private Vector2 GetDirection(ProtoPacket packet)
    {
        Vector2 vector = Vector2.zero;
        if (packet.ReadBool())
            vector -= Vector2.right;
        if (packet.ReadBool())
            vector += Vector2.right;
        if (packet.ReadBool())
            vector += Vector2.up;
        if (packet.ReadBool())
            vector -= Vector2.up;
        return vector;
    }

    private void MovePlayer(PlayerInfo info)
    {
        lock (lockObj)
        {
            foreach (var player in players)
            {
                if (player.playerGuid == Guid.ParseExact(info.Guid, "N"))
                {
                    AddFixedEvent(nameof(MovePlayer), () =>
                    {
                        Vector2 clientPos = new Vector2(info.Position.X, info.Position.Y);
                        Vector2 direction = new Vector2(info.Direction.X, info.Direction.Y);
                        player.MoveLocal(direction);
                        player.RefreshAnimation(direction);
                    });
                    break;
                }
            }
        }
    }

    private void AddEvent(string name, Action action)
    {
        lock (lockObj)
            UpdateDispatcher.Enqueue(new NetworkEvent(name, action));
    }

    private void AddFixedEvent(string name, Action action)
    {
        lock (lockObj)
            FixedUpdateDispatcher.Enqueue(new NetworkEvent(name, action));
    }

    public void ConnectAsClient(TMPro.TMP_InputField addressText)
    {
        string[] addrParts = addressText.text.Split(':');
        int port;
        try
        {
            if (addrParts.Length != 2 || !int.TryParse(addrParts[1], out port) || port > ushort.MaxValue) throw new Exception();
        }
        catch (Exception)
        {
            addressText.text = string.Empty;
            return;
        }
        ConnectAsClient(addrParts[0], port);
    }
    public async void ConnectAsClient(string ip, int port)
    {
        await Task.Run(() =>
        {
            try
            {
                Debug.Log($"Try to connect to {ip}:{port}");
                Socket.Connect(ip, port);
                networkType = NetworkType.Client;
                ListenClient(Socket);
                AddEvent(string.Empty, () => LoadNetworkScene("city"));
                SendMessageTo(Socket, new PlayerInfo());
            }
            catch (Exception e) { Debug.LogError(e.Message); }
        });
    }

    public void SendMessageTo(Guid guid, PlayerInfo info)
    {
        lock (lockObj)
        {
            foreach (var player in playersSocket)
            {
                if (player.Value.playerGuid == guid)
                {
                    SendMessageTo(player.Key, info);
                    return;
                }
            }
        }
    }

    public void SendMessageTo(Socket socket, PlayerInfo info)
    {
        MemoryStream ms = new MemoryStream();
        ms.Write(BitConverter.GetBytes(info.CalculateSize()), 0, sizeof(int));
        info.WriteTo(ms);
        try
        {
            socket.Send(ms.ToArray());
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void SendHostPlayerMovement(Vector2 direction)
    {
        lock (lockObj)
        {
            PlayerInfo info = new PlayerInfo
            {
                Type = MessageType.PlayerMoves,
                Guid = localPlayer.playerGuid.ToString("N"),
                Direction = new vec2f { X = direction.x, Y = direction.y },
                Position = new vec2f { X = localPlayer.transform.position.x, Y = localPlayer.transform.position.y }
            };
            foreach (var player in playersSocket)
            {
                SendMessageTo(player.Key, info);
            }
        }
    }

    private void CreateNewPlayer(Vector2 startPos, Socket socket = null)
    {
        PlayerController newPlayer = Instantiate(localPlayer, localPlayer.transform.parent);
        newPlayer.SetIsOnlinePlayer(true);
        newPlayer.transform.position = startPos;
        lock (lockObj)
        {
            if (socket == null)
            {
                players.Add(newPlayer);
            }
            else
                playersSocket.Add(socket, newPlayer);
        }
    }

    private void CreateNewPlayer(Vector2 startPos, Guid playerGuid, Socket socket = null)
    {
        PlayerController newPlayer = Instantiate(localPlayer, localPlayer.transform.parent);
        newPlayer.name = playerGuid.ToString("N");
        newPlayer.SetIsOnlinePlayer(true);
        newPlayer.transform.position = startPos;
        newPlayer.playerGuid = playerGuid;
        lock (lockObj)
        {
            if (socket == null)
            {
                //newPlayer.GetComponent<BoxCollider2D>().enabled = false;
                players.Add(newPlayer);
            }
            else
                playersSocket.Add(socket, newPlayer);
        }
    }

    private void LoadNetworkScene(string sceneName, PlayerInfo info)
    {
        localPlayer.gameObject.SetActive(false);
        sceneManager.SetScene(sceneName);
        languageManager.LoadTextFlags(sceneManager.GetCurrentScene());
        sceneManager.GetCurrentScene().GetComponentInChildren<LevelOneScene>().enabled = false;
        sceneManager.GetCurrentScene().GetComponentsInChildren<Animator>(true).First(an => an.gameObject.name == "Trash").enabled = false;
        localPlayer.transform.position = new Vector2(info.Position.X, info.Position.Y);
        localPlayer.playerGuid = Guid.ParseExact(info.Guid, "N");
        localPlayer.gameObject.SetActive(true);
    }
    private void LoadNetworkScene(string sceneName)
    {
        localPlayer.gameObject.SetActive(false);
        sceneManager.SetScene(sceneName);
        languageManager.LoadTextFlags(sceneManager.GetCurrentScene());
        sceneManager.GetCurrentScene().GetComponentInChildren<LevelOneScene>().enabled = false;
        sceneManager.GetCurrentScene().GetComponentsInChildren<Animator>(true).First(an => an.gameObject.name == "Trash").enabled = false;
        localPlayer.transform.position = new Vector2(-20.7f, 1.37f);
        localPlayer.gameObject.SetActive(true);
    }
    private void SendAllPlayersInfoTo(Socket client)
    {
#if !UNITY_SERVER
        vec2f hostpos = new vec2f() { X = localPlayer.transform.position.x, Y = localPlayer.transform.position.y };
        SendMessageTo(client, new PlayerInfo { Type = MessageType.NewPlayerInfo, Guid = localPlayer.playerGuid.ToString("N"), Position = hostpos });
        Debug.Log("Send host info");
#endif
        if (playersSocket.Count == 0) return;
        lock (lockObj)
        {
            foreach (var player in playersSocket)
            {
                if (player.Key == client) continue;
                vec2f pos = new vec2f { X = player.Value.transform.position.x, Y = player.Value.transform.position.y };
                SendMessageTo(client, new PlayerInfo { Type = MessageType.NewPlayerInfo, Guid = player.Value.playerGuid.ToString("N"), Position = pos });
                Debug.Log("Send player info about " + player.Value.playerGuid);
            }
        }
    }
    private void SendConnectedPlayerToOthers(Socket client, PlayerInfo info)
    {
        if (playersSocket.Count == 0) return;
        lock (lockObj)
        {
            foreach (var player in playersSocket)
            {
                if (client == player.Key) continue;
                SendMessageTo(player.Key, new PlayerInfo(info) { Type = MessageType.NewPlayerConnected });
            }
        }
    }
    private void SendPlayerMovementToAll(Socket client, PlayerInfo info)
    {
        lock (lockObj)
        {
            foreach (var player in playersSocket)
            {
                if (client == player.Key) continue;
                SendMessageTo(player.Key, info);
            }
        }
    }
    private void RemovePlayer(Guid guid)
    {
        lock (lockObj)
        {
            var player = players.Find(p => p.playerGuid == guid);
            Destroy(player.gameObject);
            players.Remove(player);
        }
    }
    private void RemovePlayer(Socket client)
    {
        switch (networkType)
        {
            case NetworkType.Client:
                RemovePlayerFromClient(client);
                break;
            case NetworkType.Server:
                RemovePlayerFromServer(client);
                break;
        }
        Debug.Log($"Stop listening {client.RemoteEndPoint}");
        client.Close();
        client.Dispose();
    }
    private void RemovePlayerFromServer(Socket client)
    {
        Guid guid = playersSocket[client].playerGuid;
        Destroy(playersSocket[client].gameObject);
        playersSocket.Remove(client);
        lock (lockObj)
        {
            foreach (var player in playersSocket)
            {
                SendMessageTo(player.Key, new PlayerInfo { Type = MessageType.PlayerDisconnected, Guid = guid.ToString("N") });
            }
        }
    }
    private void RemovePlayerFromClient(Socket client)
    {
        players.ForEach(p => Destroy(p.gameObject));
        players.Clear();
        sceneManager.SetScene("mainmenu");
        languageManager.LoadTextFlags(sceneManager.GetCurrentScene());
        networkType = NetworkType.None;
    }
    public void RefreshPlayerLayers()
    {
        switch (networkType)
        {
            case NetworkType.Server:
                RefreshServerSidePlayers();
                break;
            case NetworkType.Client:
                RefreshClientSidePlayers();
                break;
        }
    }
    private void RefreshClientSidePlayers()
    {
        if (players.Count < 1) return;
        PlayerController lowMagnitudePlayer = null;
        float lowDist = 0;
        lock (lockObj)
        {
            foreach (var player in players)
            {
                float dist = Vector2.Distance(localPlayer.transform.position, player.transform.position);
                if (!lowMagnitudePlayer)
                {
                    lowDist = dist;
                    lowMagnitudePlayer = player;
                    continue;
                }
                if (dist < lowDist)
                {
                    lowDist = dist;
                    lowMagnitudePlayer = player;
                }
            }
        }
        if (localPlayer.transform.position.y < lowMagnitudePlayer.transform.position.y)
        {
            localPlayer.GetComponent<SpriteRenderer>().sortingOrder = 16;
            lowMagnitudePlayer.GetComponent<SpriteRenderer>().sortingOrder = 15;
        }
        else
        {
            localPlayer.GetComponent<SpriteRenderer>().sortingOrder = 15;
            lowMagnitudePlayer.GetComponent<SpriteRenderer>().sortingOrder = 16;
        }
    }
    private void RefreshServerSidePlayers()
    {
        if (playersSocket.Count < 1) return;
        PlayerController lowMagnitudePlayer = null;
        float lowDist = 0;
        lock (lockObj)
        {
            foreach (var player in playersSocket)
            {
                float dist = Vector2.Distance(localPlayer.transform.position, player.Value.transform.position);
                if (!lowMagnitudePlayer)
                {
                    lowDist = dist;
                    lowMagnitudePlayer = player.Value;
                    continue;
                }
                if (dist < lowDist)
                {
                    lowDist = dist;
                    lowMagnitudePlayer = player.Value;
                }
            }
        }
        if (localPlayer.transform.position.y < lowMagnitudePlayer.transform.position.y)
        {
            localPlayer.GetComponent<SpriteRenderer>().sortingOrder = 16;
            lowMagnitudePlayer.GetComponent<SpriteRenderer>().sortingOrder = 15;
        }
        else
        {
            localPlayer.GetComponent<SpriteRenderer>().sortingOrder = 15;
            lowMagnitudePlayer.GetComponent<SpriteRenderer>().sortingOrder = 16;
        }
    }
}
