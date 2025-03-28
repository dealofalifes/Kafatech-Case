using UnityEngine;
using System.Collections.Generic;

public class NavigationController : MonoBehaviour, INavigationController
{
    public static NavigationController Instance { get; private set; }
    public void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public List<NavigationPoint> CreatePath(NavigationPoint _start, NavigationPoint _end)
    {
        return AStarPathfinding(_start, _end);
    }

    private List<NavigationPoint> AStarPathfinding(NavigationPoint _start, NavigationPoint _end)
    {
        List<NavigationPoint> openSet = new List<NavigationPoint> { _start };
        HashSet<NavigationPoint> closedSet = new HashSet<NavigationPoint>();

        Dictionary<NavigationPoint, NavigationPoint> cameFrom = new Dictionary<NavigationPoint, NavigationPoint>();
        Dictionary<NavigationPoint, float> gScore = new Dictionary<NavigationPoint, float>
        {
            [_start] = 0
        };
        Dictionary<NavigationPoint, float> fScore = new Dictionary<NavigationPoint, float>
        {
            [_start] = Vector3.Distance(_start.transform.position, _end.transform.position)
        };

        while (openSet.Count > 0)
        {
            NavigationPoint current = GetLowestFScoreNode(openSet, fScore);
            //Debug.Log($"Current Node: {current.PathName} at Position: {current.Position}");

            if (current == _end)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighborPosition in current.Connections)
            {
                NavigationPoint neighbor = neighborPosition;

                if (neighbor == null)
                {
                    Debug.LogError($"Waypoint {current.transform.name} has a null connection or invalid neighbor position.");
                    continue;
                }

                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor.transform.position, _end.transform.position);

                //Debug.Log($"Neighbor: {neighbor.PathName}, Tentative GScore: {tentativeGScore}, FScore: {fScore[neighbor]}");
            }

            // Debugging the state of openSet and closedSet
            //Debug.Log($"Open Set Count: {openSet.Count}");
            //Debug.Log($"Closed Set Count: {closedSet.Count}");
        }

        //Debug.LogWarning("Path not found! Returning partial path.");
        return ReconstructPath(cameFrom, GetFurthestNode(cameFrom, _start));
    }

    private NavigationPoint GetFurthestNode(Dictionary<NavigationPoint, NavigationPoint> cameFrom, NavigationPoint start)
    {
        NavigationPoint furthestNode = start;
        float maxDistance = 0;

        foreach (var node in cameFrom.Keys)
        {
            float distance = Vector3.Distance(start.transform.position, node.transform.position);
            if (distance > maxDistance)
            {
                furthestNode = node;
                maxDistance = distance;
            }
        }

        //Debug.Log($"Furthest Node: {furthestNode.PathName}");
        return furthestNode;
    }

    private List<NavigationPoint> ReconstructPath(Dictionary<NavigationPoint, NavigationPoint> cameFrom, NavigationPoint current)
    {
        List<NavigationPoint> totalPath = new List<NavigationPoint> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }

        // Debugging the reconstructed path
        //Debug.Log("Reconstructed Path:");
        foreach (var waypoint in totalPath)
        {
            //Debug.Log($"Waypoint: {waypoint.PathName} at Position: {waypoint.Position}, Connections: {waypoint.connections.Count}");
        }

        return totalPath;
    }

    private NavigationPoint GetLowestFScoreNode(List<NavigationPoint> openSet, Dictionary<NavigationPoint, float> fScore)
    {
        NavigationPoint lowest = openSet[0];
        float lowestScore = fScore.GetValueOrDefault(lowest, float.MaxValue);

        foreach (var node in openSet)
        {
            float score = fScore.GetValueOrDefault(node, float.MaxValue);
            if (score < lowestScore)
            {
                lowest = node;
                lowestScore = score;
            }
        }

        //Debug.Log($"Lowest FScore Node: {lowest.PathName}, Score: {lowestScore}");
        return lowest;
    }
}

public interface INavigationController
{
    public List<NavigationPoint> CreatePath(NavigationPoint _start, NavigationPoint _end);
}
