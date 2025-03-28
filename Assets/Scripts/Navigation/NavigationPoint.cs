using System.Collections.Generic;
using UnityEngine;

public class NavigationPoint : MonoBehaviour
{
    public List<NavigationPoint> Connections;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        if (Connections != null)
        {
            foreach (var connection in Connections)
            {
                Gizmos.DrawLine(transform.position, connection.transform.position);
            }
        }
    }
#endif
}
