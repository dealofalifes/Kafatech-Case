using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private ICombatRPCHandler _CombatRPCHandler;

    [SerializeField] private Transform MyBuildingsContainer;
    [SerializeField] private Transform EnemyBuildingsContainer;
    [SerializeField] private Transform MySoldiersContainer;
    [SerializeField] private Transform EnemySoldiersContainer;
    public void Awake()
    {
        if (_CombatRPCHandler == null)
            _CombatRPCHandler = GameManager.Instance.GetCombatRPCHandler();

        _CombatRPCHandler.OnEndDamage += OnEndDamage;
    }

    private void OnDisable()
    {
        _CombatRPCHandler.OnEndDamage -= OnEndDamage;
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                if (hit.collider != null && hit.collider.TryGetComponent(out ISelectable _selectable))
                {
                    // The object was touched
                    OnTouched(hit.collider.transform);
                }
            }
        }
    }

    void OnTouched(Transform _hit)
    {
        Debug.Log(_hit.gameObject.name + " touched!");
        // Your custom function here

        if(_hit.TryGetComponent(out IDamagable target))
            ApplyDamageToBuilding(target);
    }

    void ApplyDamageToBuilding(IDamagable _target)
    {
        int exampleDamage = 10;
        _CombatRPCHandler.ApplyDamageBuilding(_target.GetBuildingID(), exampleDamage);
    }

    public void OnEndDamage(int _targetType, int _damageAmount, int _targetId, int _finalHealth, bool _damaged)
    {
        //if damaged is true, other player gave this damage to you.

        if ((TargetType)_targetType == TargetType.Building)
        {
            Transform target = _damaged ? (MyBuildingsContainer.GetChild(_targetId - 1)) : 
                (EnemyBuildingsContainer.GetChild(_targetId - 1));

            if (target.TryGetComponent(out IDamagable _damagable))
                _damagable.UpdateHealth(_finalHealth);
        }
        else if ((TargetType)_targetType == TargetType.Soldier)
        {
            Transform target = _damaged ? (MySoldiersContainer.GetChild(_targetId - 1)) :
                (EnemySoldiersContainer.GetChild(_targetId - 1));
        }
    }
}
