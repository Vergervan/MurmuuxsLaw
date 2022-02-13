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
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public Socket Socket { get => socket; }
    private MemoryStream messageStream;
    SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
    private static Queue<Action> UpdateDispatcher = new Queue<Action>();
    private static Queue<Action> FixedUpdateDispatcher = new Queue<Action>();
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
        if (Input.GetKeyDown(KeyCode.Space) && networkType == NetworkType.Client)
            SendMessageTo(socket, new PlayerInfo
            {
                Type = MessageType.Connect
            });
        while (UpdateDispatcher.Count > 0) UpdateDispatcher.Dequeue().Invoke();
        RefreshPlayerLayers();

    }
    private void FixedUpdate()
    {
        while (FixedUpdateDispatcher.Count > 0) FixedUpdateDispatcher.Dequeue().Invoke();
    }
    private void OnDestroy()
    {
        messageStream?.Close();
        messageStream?.Dispose();
        if (socket.Connected)
            socket.Shutdown(SocketShutdown.Both);
        socket?.Close();
        socket?.Dispose();
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
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#if UNITY_SERVER
        socket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
#else
        if (!port.HasValue || port > ushort.MaxValue) return;
        socket.Bind(new IPEndPoint(IPAddress.Any, port.Value));
#endif
        socket.Listen(0);
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
        socket.AcceptAsync(args);
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
            byte[] buffer = new byte[4];
            byte[] protoBuffer;
            int size = 0;
            int protoMsgSize = 0;
            do
            {
                try
                {
                    size = client.Receive(buffer);
                    if (size < 4)
                    {
                        Debug.Log($"0 bytes by {client.RemoteEndPoint}");
                        UpdateDispatcher.Enqueue(() => RemovePlayerFromServer(client));
                        break;
                    }
                    protoMsgSize = BitConverter.ToInt32(buffer, 0);
                    PlayerInfo info;
                    if (protoMsgSize == 0) info = new PlayerInfo();
                    else
                    {
                        protoBuffer = new byte[protoMsgSize];
                        client.Receive(protoBuffer);
                        info = PlayerInfo.Parser.ParseFrom(protoBuffer);
                    }
                    Task.Run(() => ProcessMessage(client, info));
                }
                catch (SocketException e)
                {
                    Debug.LogError(e.Message);
                }
            } while (true);
        });
    }
    private void ProcessMessage(Socket client, PlayerInfo info)
    {
        Debug.Log("Process");
        Vector2 startPos = new Vector2(-20.7f, 1.37f);
        switch (info.Type)
        {
            //SERVER SIDE MESSAGES
            case MessageType.Connect:
                Debug.Log($"{client.RemoteEndPoint}: Connected");
                Guid playerGuid = Guid.NewGuid();
                UpdateDispatcher.Enqueue(() => CreateNewPlayer(startPos, playerGuid, client));
                PlayerInfo infoReq = new PlayerInfo { Type = MessageType.AcceptConnect, Position = new vec2f { X = startPos.x, Y = startPos.y }, Guid = playerGuid.ToString("N") };
                SendMessageTo(client, infoReq);
                SendConnectedPlayerToOthers(client, infoReq);
                break;

            case MessageType.InfoRequest:
                UpdateDispatcher.Enqueue(() => SendAllPlayersInfoTo(client));
                break;

            case MessageType.Move:
                //playersSocket[client].RememberOldPos();
                info.Type = MessageType.AcceptMove;
                FixedUpdateDispatcher.Enqueue(() => playersSocket[client].MoveLocal(new Vector2(info.Direction.X, info.Direction.Y)));
                SendMessageTo(client, info);
                PlayerInfo info2 = new PlayerInfo(info);
                info2.Type = MessageType.PlayerMoves;
                SendPlayerMovementToAll(client, info2);
                break;

            //CLIENT SIDE MESSAGES
            case MessageType.AcceptConnect:
                UpdateDispatcher.Enqueue(() => LoadNetworkScene("city", info));
                SendMessageTo(socket, new PlayerInfo { Type = MessageType.InfoRequest });
                break;
            case MessageType.NewPlayerInfo:
                UpdateDispatcher.Enqueue(() => CreateNewPlayer(new Vector2(info.Position.X, info.Position.Y), Guid.ParseExact(info.Guid, "N")));
                break;

            case MessageType.AcceptMove:
                FixedUpdateDispatcher.Enqueue(() => localPlayer.MoveLocal(new Vector2
                { x = info.Direction.X, y = info.Direction.Y }));
                break;
            case MessageType.NewPlayerConnected:
                UpdateDispatcher.Enqueue(() => CreateNewPlayer(new Vector2(info.Position.X, info.Position.Y), Guid.ParseExact(info.Guid, "N")));
                break;
            case MessageType.PlayerMoves:
                lock (lockObj)
                {
                    foreach (var player in players)
                    {
                        if (player.playerGuid == Guid.ParseExact(info.Guid, "N"))
                        {
                            FixedUpdateDispatcher.Enqueue(() => player.MoveLocal(new Vector2(info.Direction.X, info.Direction.Y)));
                            break;
                        }
                    }
                }
                break;
            case MessageType.PlayerDisconnected:
                UpdateDispatcher.Enqueue(() => RemovePlayer(Guid.ParseExact(info.Guid, "N")));
                break;
        }
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
                socket.Connect(ip, port);
                networkType = NetworkType.Client;
                ListenClient(socket);
                SendMessageTo(socket, new PlayerInfo());
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

    public async void SendMessageTo(Socket socket, PlayerInfo info)
    {
        await Task.Run(() =>
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
        });
    }

    public void SendHostPlayerMovement(Vector2Int direction)
    {
        lock (lockObj)
        {
            PlayerInfo info = new PlayerInfo { Type = MessageType.PlayerMoves, Guid = localPlayer.playerGuid.ToString("N"), Direction = new vec2 { X = direction.x, Y = direction.y } };
            foreach (var player in playersSocket)
            {
                SendMessageTo(player.Key, info);
            }
        }
    }

    private void CreateNewPlayer(Vector2 startPos, Guid playerGuid, Socket socket = null)
    {
        PlayerController newPlayer = Instantiate(localPlayer, localPlayer.transform.parent);
        newPlayer.name = playerGuid.ToString("N");
        newPlayer.SetIsOnlinePlayer(true);
        newPlayer.transform.position = startPos;
        newPlayer.playerGuid = playerGuid;
        if (socket == null)
        {
            //newPlayer.GetComponent<BoxCollider2D>().enabled = false;
            players.Add(newPlayer);
        }
        else
            playersSocket.Add(socket, newPlayer);
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

    private void SendAllPlayersInfoTo(Socket client)
    {
        lock (lockObj)
        {
            vec2f pos;
#if !UNITY_SERVER
            pos = new vec2f { X = localPlayer.transform.position.x, Y = localPlayer.transform.position.y };
            SendMessageTo(client, new PlayerInfo { Type = MessageType.NewPlayerInfo, Guid = localPlayer.playerGuid.ToString("N"), Position = pos });
#endif
            foreach (var player in playersSocket)
            {
                if (player.Key == client) continue;
                pos = new vec2f { X = player.Value.transform.position.x, Y = player.Value.transform.position.y };
                SendMessageTo(client, new PlayerInfo { Type = MessageType.NewPlayerInfo, Guid = player.Value.playerGuid.ToString("N"), Position = pos });
            }
        }
    }
    private void SendConnectedPlayerToOthers(Socket client, PlayerInfo info)
    {
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
            foreach (var player in players)
            {
                if (player.playerGuid == guid)
                {
                    Destroy(player.gameObject);
                    players.Remove(player);
                    return;
                }
            }
        }
    }
    private void RemovePlayerFromServer(Socket client)
    {
        lock (lockObj)
        {
            Guid guid = playersSocket[client].playerGuid;
            Destroy(playersSocket[client].gameObject);
            playersSocket.Remove(client);
            client.Close();
            client.Dispose();
            foreach (var player in playersSocket)
            {
                SendMessageTo(player.Key, new PlayerInfo { Type = MessageType.PlayerDisconnected, Guid = guid.ToString("N") });
            }
            Debug.Log("Stop listening");
        }
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
