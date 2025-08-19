using AF;
using UnityEngine;
using UnityEngine.AI;

public class DummyAIAgent : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;

    [SerializeField] bool useDistance = false;
    [SerializeField] bool useCalculatePath = false;

    NavMeshAgent agent => GetComponent<NavMeshAgent>();

    private void Update()
    {
        if (useDistance)
        {
            agent.SetDestination(playerManager.transform.position);
        }
        else if (useCalculatePath)
        {

            NavMeshPath navMeshPath = new();
            agent.CalculatePath(playerManager.transform.position, navMeshPath);
            agent.SetPath(navMeshPath);
        }

    }
}