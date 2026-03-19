using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private Enemy enemy;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        Transform target = enemy.GetTarget();
        if (enemy.canAct && target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
        else
        {
            navMeshAgent.ResetPath();
        }
    }

}
