using FishNet.Object;
using FishNet.Connection;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Managing.Scened;
using FishNet;
using System.Collections;
using System;

public class Matchmaker : NetworkBehaviour
{
    [SerializeField] private List<NetworkConnection> _Queue = new List<NetworkConnection>();
    [SerializeField] private Pairs _Pairs = new();

    public event Action<Pairs> OnCreatedRoom;
    public void JoinLobby(NetworkConnection _conn) //Observer request to join for queue
    {
        JoinLobby_RPC(_conn);
    }

    [Server]
    private void JoinLobby_RPC(NetworkConnection _conn)
    {
        bool contains = false;
        int length = _Queue.Count;
        for (int i = length - 1; i >= 0; i--)
        {
            if (_Queue[i].ClientId == _conn.ClientId)
            {
                contains = true;
                break;
            }
        }

        if (!contains)
        {
            _Queue.Add(_conn);
            if (_Queue.Count >= 2)
            {
                StartMatch();
            }
        }
    }

    public void ExitLobby(NetworkConnection _conn) //Observer disconnected or exitted.
    {
        ExitLobby_RPC(_conn);
    }

    [Server]
    private void ExitLobby_RPC(NetworkConnection _conn)
    {
        int QueueLength = _Queue.Count;
        for (int i = QueueLength - 1; i >= 0; i--)
        {
            if (_Queue[i].ClientId == _conn.ClientId)
            {
                _Queue.RemoveAt(i);
                //Do something.
            }
        }

        List<ConnectionPair> Pairs = _Pairs.GetPairs();
        int PairLength = _Pairs.Count;
        for (int i = PairLength - 1; i >= 0; i--)
        {
            if (Pairs[i].IsPair(_conn.ClientId))
            {
                var _connections = Pairs[i].GetConnections();

                InstanceFinder.ServerManager.Kick(_connections._p1, FishNet.Managing.Server.KickReason.Unset);
                InstanceFinder.ServerManager.Kick(_connections._p2, FishNet.Managing.Server.KickReason.Unset);

                _Pairs.RemovePairByIndex(i);
                //Do something.
            }
        }
    }

    private void StartMatch()
    {
        var player1 = _Queue[0];
        var player2 = _Queue[1];

        ConnectionPair newPair = new(player1, player2);
        _Pairs.AddPair(newPair);
        _Queue.RemoveRange(0, 2);

        // Create a match session
        StartCoroutine(CreateRoom(newPair));
    }

    IEnumerator CreateRoom(ConnectionPair _newPair)
    {
        yield return new WaitForSeconds(3);

        bool loadScene = false;
        int PairLength = _Pairs.Count;
        var connections = _newPair.GetConnections();
        List<ConnectionPair> Pairs = _Pairs.GetPairs();
        for (int i = PairLength - 1; i >= 0; i--)
        {
            if (Pairs[i].IsPair(connections._p1.ClientId)) //After 3 seconds later, pairs are still online or not
            {
                NetworkConnection[] conns = new NetworkConnection[] { connections._p1, connections._p2 };
                SceneLoadData sld = new SceneLoadData("GameScene");
                sld.ReplaceScenes = ReplaceOption.None;

                Debug.Log("Matching is created. player IDs: " + connections._p1.ClientId + " / " + connections._p2.ClientId);
                InstanceFinder.SceneManager.LoadConnectionScenes(conns, sld);

                if(OnCreatedRoom != null)
                    OnCreatedRoom.Invoke(_Pairs);

                loadScene = true;
                break;
            }
        }

        if (!loadScene)
        {
            Debug.Log("Pair disconnected.");
            InstanceFinder.ServerManager.Kick(connections._p1, FishNet.Managing.Server.KickReason.Unset);
            InstanceFinder.ServerManager.Kick(connections._p2, FishNet.Managing.Server.KickReason.Unset);
        }                                     
    }

    public void EndGame()
    {

    }
}