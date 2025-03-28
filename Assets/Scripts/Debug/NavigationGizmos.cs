#if UNITY_EDITOR

using UnityEngine;

public class NavigationGizmos : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        foreach (Transform point in transform)
        {
            NavigationPoint currentPoint = point.GetComponent<NavigationPoint>();
            if (currentPoint.Connections != null)
            {
                foreach (var connection in currentPoint.Connections)
                {
                    Gizmos.DrawLine(point.position, connection.transform.position);
                }
            }
        }
    }
}
#endif