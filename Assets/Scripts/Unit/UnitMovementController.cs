using UnityEngine;
using UnityEngine.AI;

public class UnitMovementController : MonoBehaviour, IAction
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] float maxUnitSpeed = 1f;
    [SerializeField] float maxNavPathLength = 40f;

    private ActionScheduler actionScheduler;
    private bool isInitialized;

    private void Awake()
    {
        actionScheduler = new();

        isInitialized = true;
    }

    public void StartMoveAction(Vector3 destination, float speedFraction)
    {
        actionScheduler.StartAction(this);
        MoveTo(destination, speedFraction);
    }

    public void MoveTo(Vector3 destination, float speedFraction)
    {
        navMeshAgent.destination = destination;
        navMeshAgent.speed = maxUnitSpeed * Mathf.Clamp01(speedFraction);
        navMeshAgent.isStopped = false;
    }

    public bool CanMoveTo(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        bool CanMoveInDistance = GetPathLength(path) > maxNavPathLength;

        return hasPath && CanMoveInDistance;
    }

    public void Cancel()
    {
        navMeshAgent.isStopped = true;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float pathLength = 0f;
        if (path.corners.Length < 2) return pathLength;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Sum all distances from corner to another corner
            pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return pathLength;
    }
}
