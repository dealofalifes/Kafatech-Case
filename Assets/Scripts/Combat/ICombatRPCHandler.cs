using System;
using UnityEngine;

public interface ICombatRPCHandler
{
    public void ApplyDamageBuilding(int _targetBuilding, int _damage);

    //Params: int _targetType, int _damageAmount, int _targetId, int _finalHealth, int _clientID
    public event Action<int, int, int, int, bool> OnEndDamage;
}
