using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetworkManager
{
    public static NetworkManager Instance { get; private set; }
    [SerializeField] private bool _ServerBuild;
    [SerializeField] private bool _Connected;

    [SerializeField] private Matchmaker _Matchmaker;
    [SerializeField] private CombatRPCHandler _CombatRPCHandler;

    public event Action OnClientConnected;
    public event Action OnClientDisconnected;

    public event Action<int> OnRemoteConnected;
    public event Action<int> OnRemoteDisconnected;

    public event Action OnServerInit;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (_ServerBuild)
        {
            if(OnServerInit != null)
                OnServerInit.Invoke();

            StartCoroutine(Connect());

            InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
            InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionState;

            _Matchmaker.OnCreatedRoom += OnCreatedRoom;
        }
        
    }

    IEnumerator Connect()
    {
        while (true)
        {
            if (!_Connected)
            {
                InstanceFinder.ServerManager.StartConnection();
                yield return new WaitForSeconds(5);
            }
            else
            {
                yield return new WaitForSeconds(25);
            }
        }
    }

    private void OnDisable()
    {
        InstanceFinder.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
        InstanceFinder.ServerManager.OnServerConnectionState -= OnServerConnectionState;
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;

        _Matchmaker.OnCreatedRoom -= OnCreatedRoom;
    }

    public void ClientJoinLobby()
    {
        InstanceFinder.ClientManager.StartConnection();

        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
    }

    public void ClientExitLobby()
    {
        InstanceFinder.ClientManager.StopConnection();
    }

    private void OnRemoteConnectionState(NetworkConnection _conn, RemoteConnectionStateArgs _args)
    {
        Debug.Log("OnRemoteConnectionState");
        if (_args.ConnectionState == RemoteConnectionState.Stopped)
        {
            Debug.Log($"Player with ID {_conn.ClientId} disconnected from the server.");
            _Matchmaker.ExitLobby(_conn);
            if(OnRemoteDisconnected != null)
                OnRemoteDisconnected.Invoke(_conn.ClientId);
            //_MainMenuController.UpdateInformationText($"Player with ID {_conn.ClientId} disconnected from the server.");
        }
        else if (_args.ConnectionState == RemoteConnectionState.Started)
        {
            Debug.Log($"Player with ID {_conn.ClientId} connected to the server.");
            _Matchmaker.JoinLobby(_conn);
            if (OnRemoteConnected != null)
                OnRemoteConnected.Invoke(_conn.ClientId);
            //_MainMenuController.UpdateInformationText($"Player with ID {_conn.ClientId} connected to the server.");
        }
    }

    private void OnServerConnectionState(ServerConnectionStateArgs _args)
    {
        Debug.Log("OnServerConnectionState");
        if (_args.ConnectionState == LocalConnectionState.Stopped)
        {
            Debug.Log($"Server stopped!");
            //_MainMenuController.UpdateInformationText("Server Stopped!");
        }
        else if (_args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log($"Server Started!");
            //_MainMenuController.UpdateInformationText("Server Started!");
        }
    }

    private void OnClientConnectionState(ClientConnectionStateArgs _args)
    {
        Debug.Log("OnClientConnectionState"); //When client joins, this function will be triggered at 'CLIENT' side
        if (_args.ConnectionState == LocalConnectionState.Stopped)
        {
            Debug.Log($"Client stopped!");
            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
            OnClientDisconnected.Invoke();
        }
        else if (_args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log($"Client Started!");
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
            OnClientConnected.Invoke();
        }
    }

    public void OnCreatedRoom(Pairs _newPair)
    {
        _CombatRPCHandler.UpdatePairs(_newPair);
    }
}

public interface INetworkManager
{
    public event Action OnServerInit;
    public event Action OnClientConnected;
    public event Action OnClientDisconnected;
    public event Action<int> OnRemoteConnected;
    public event Action<int> OnRemoteDisconnected;
}