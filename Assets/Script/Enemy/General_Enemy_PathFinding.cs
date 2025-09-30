using UnityEngine;
using UnityEngine.AI;

public class General_Enemy_PathFinding
{
    private NavMeshAgent agent;
    private Transform target;

    public General_Enemy_PathFinding(NavMeshAgent agent, Transform target)
    {
        this.agent = agent;
        this.target = target;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void ChaseTarget()
    {
        if (agent != null && target != null)
            agent.SetDestination(target.position);
    }
}