using FishNet;
using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

public class CombatRPCHandler : NetworkBehaviour, ICombatRPCHandler
{
    //Params: int _targetType, int _damageAmount, int _targetId, int _finalHealth, int _clientID
    public event Action<int, int, int, int, bool> OnEndDamage;

    [SerializeField] private Pairs _Pairs;
    public void UpdatePairs(Pairs _currentPairs)
    {
        _Pairs = _currentPairs;
    }

    public void ApplyDamageBuilding(int _targetBuilding, int _damage)
    {
        RequestDamage_ServerRpc((int)TargetType.Building, _damage, _targetBuilding, InstanceFinder.ClientManager.Connection);
    }

    /// <summary>
    /// Client sends damage request to server.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void RequestDamage_ServerRpc(int _targetType, int _damageAmount, int _targetId, NetworkConnection sender = null)
    {
        if (sender == null)
        {
            Debug.LogError("Who sent the RPC?");
            return;
        }

        if (_targetId <= 0)
        {
            Debug.LogError("Invalid targetID with: " + _targetId);
            return;
        }

        var pair = _Pairs.GetConnectionGameDatas(sender.ClientId);

        ConnectionGameData owner = null;
        ConnectionGameData target = null;

        if (sender.ClientId == pair._p1.GetClientID())
        {
            Debug.Log("Player1 attacks to player2");
            owner = pair._p1;
            target = pair._p2;
        }
        else if (sender.ClientId == pair._p2.GetClientID())
        {
            Debug.Log("Player2 attacks to player1");
            owner = pair._p2;
            target = pair._p1;
        }

        int finalHealth = 0;
        if ((TargetType)_targetType == TargetType.Soldier) //Soldier Index
        {
            finalHealth = target.ApplyDamageToSoldier(_targetId - 1, _damageAmount);
        }
        else if((TargetType)_targetType == TargetType.Building) //Building Index
        {
            finalHealth = target.ApplyDamageToBuilding(_targetId - 1, _damageAmount);
        }

        Debug.Log($"Server: {sender.ClientId} dealt {_damageAmount} to {(TargetType)_targetType} ID: {_targetId}. New HP: {finalHealth}");

        // Notify all clients about the updated health
        BroadcastHealth_ClientRpc(_targetType, _damageAmount, _targetId, finalHealth, sender.ClientId == target.GetClientID());
    }

    /// <summary>
    /// Server notifies all clients of updated health.
    /// </summary>
    [ObserversRpc]
    private void BroadcastHealth_ClientRpc(int _targetType, int _damageAmount, int _targetId, int _finalHealth, bool _damaged)
    {
        Debug.Log($"Damage Owner ({_damaged}) target {_targetType} ID {_targetId} got damage amount {_damageAmount}. now has {_finalHealth} HP.");
        OnEndDamage.Invoke(_targetType, _damageAmount, _targetId, _finalHealth, _damaged);
    }
}

public enum TargetType
{
    Building,
    Soldier,
}