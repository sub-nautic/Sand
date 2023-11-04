using UnityEngine;
using UnityEngine.AI;

public class UnitMovementController : MonoBehaviour, IAction
{
    [Tooltip("Reference to the unit NavMeshAgent component")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [Tooltip("Maximum unit movement speed")]
    [SerializeField] float maxUnitSpeed = 1f;
    [Tooltip("Maximum allowed navigation path length")]
    [SerializeField] float maxNavPathLength = 100f;

    private ActionScheduler actionScheduler;
    private bool isInitialized;

    private void Awake()
    {
        // Initialize the action scheduler
        actionScheduler = new ActionScheduler();

        isInitialized = true;
    }

    public void StartMoveAction(Vector3 destination, float speedFraction)
    {
        if (!CanMoveTo(destination))
        {
            return;
        }

        // Notify the action scheduler that a move action is starting
        actionScheduler.StartAction(this);

        // Start moving the unit to the specified destination
        MoveTo(destination, speedFraction);
    }

    public void MoveTo(Vector3 destination, float speedFraction)
    {
        navMeshAgent.destination = destination;
        navMeshAgent.speed = maxUnitSpeed * Mathf.Clamp01(speedFraction);

        // Allow the NavMeshAgent to move
        navMeshAgent.isStopped = false;
    }

    public bool CanMoveTo(Vector3 destination)
    {
        // Create a NavMeshPath for path calculation
        NavMeshPath path = new NavMeshPath();
        bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        bool CanMoveInDistance = GetPathLength(path) < maxNavPathLength;

        // Return true if there's a path and it's within the allowed distance
        return hasPath && CanMoveInDistance;
    }

    public void Cancel()
    {
        // Stop the NavMeshAgent from moving
        navMeshAgent.isStopped = true;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float pathLength = 0f;

        if (path.corners.Length < 2)
        {
            // If there are not enough corners in the path, return 0.
            return pathLength;
        }

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Sum all distances from corner to another corner to calculate the path length
            pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return pathLength;
    }
}
