using FishNet.Connection;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pairs
{
    private List<ConnectionPair> _Pairs = new();

    public int Count
    {
        get => _Pairs.Count;
    }

    public void AddPair(ConnectionPair _newPair)
    {
        _Pairs.Add(_newPair);
    }

    public void RemovePairByIndex(int _index)
    {
        _Pairs.RemoveAt(_index);
    }

    public List<ConnectionPair> GetPairs()
    {
        return _Pairs;
    }

    public (ConnectionGameData _p1, ConnectionGameData _p2) GetConnectionGameDatas(int _clientID)
    {
        foreach (var item in _Pairs)
        {
            if (item.IsPair(_clientID))
            {
                return item.GetConnectionGameDatas();
            }
        }

        return (null, null);
    }
}

[System.Serializable]
public class ConnectionPair
{
    private ConnectionGameData _Connection1;
    private ConnectionGameData _Connection2;

    public ConnectionPair(NetworkConnection _conn1, NetworkConnection _conn2)
    {
        _Connection1 = new(_conn1);
        _Connection2 = new(_conn2);
    }

    public bool IsPair(int _cliendID)
    {
        return _Connection1.GetClientID() == _cliendID || _Connection2.GetClientID() == _cliendID;
    }

    public (NetworkConnection _p1, NetworkConnection _p2) GetConnections()
    {
        return (_Connection1.GetPlayerConnection(), _Connection2.GetPlayerConnection());
    }

    public (ConnectionGameData _p1, ConnectionGameData _p2) GetConnectionGameDatas()
    {
        return (_Connection1, _Connection2);
    }
}

[System.Serializable]
public class ConnectionGameData
{
    private NetworkConnection _PlayerConnection;
    private List<int> _TowerHealths;
    private List<int> _SoldierHealths;

    public ConnectionGameData(NetworkConnection _connection)
    {
        _PlayerConnection = _connection;
        _TowerHealths = new() { 1000, 1000, 1000 };
        _SoldierHealths = new();
    }

    public int GetClientID()
    {
        return _PlayerConnection.ClientId;
    }

    public NetworkConnection GetPlayerConnection()
    {
        return _PlayerConnection;
    }

    public int ApplyDamageToSoldier(int _index, int _damage) //id - 1
    {
        _SoldierHealths[_index] -= _damage;
        if (_SoldierHealths[_index] <= 0)
        {
            _SoldierHealths.RemoveAt(_index);
            return 0;
        }

        return _SoldierHealths[_index];
    }

    public int ApplyDamageToBuilding(int _index, int _damage) //id - 1
    {
        _TowerHealths[_index] -= _damage;
        if (_TowerHealths[_index] <= 0)
        {
            _TowerHealths.RemoveAt(_index);
            return 0;
        }

        return _TowerHealths[_index];
    }
}