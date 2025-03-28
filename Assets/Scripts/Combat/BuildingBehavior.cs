using UnityEngine;
using UnityEngine.UI;

public class BuildingBehavior : MonoBehaviour, ISelectable, IDamagable
{
    [SerializeField] private int _ID;
    [SerializeField] private TextMesh _HealthTextMesh;
    public int GetBuildingID()
    {
        return _ID;
    }

    public void UpdateHealth(int _currentHealth)
    {
        _HealthTextMesh.text = "" + _currentHealth;
    }
}
