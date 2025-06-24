using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    [Tooltip("Assign your waypoints in order here.")]
    public Transform[] waypoints;

    [Tooltip("Assign your waypoints for Path 1 in order here.")]
    public Transform[] path1Waypoints;
    [Tooltip("Assign your waypoints for Path 2 in order here.")]
    public Transform[] path2Waypoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public Transform[] GetRandomPath()
    {
        if (path1Waypoints != null && path2Waypoints != null && path1Waypoints.Length > 0 && path2Waypoints.Length > 0)
        {
            return Random.value < 0.5f ? path1Waypoints : path2Waypoints;
        }
        else if (path1Waypoints != null && path1Waypoints.Length > 0)
        {
            return path1Waypoints;
        }
        else if (path2Waypoints != null && path2Waypoints.Length > 0)
        {
            return path2Waypoints;
        }
        else
        {
            Debug.LogWarning("No waypoints assigned in WaypointManager!");
            return new Transform[0];
        }
    }
}