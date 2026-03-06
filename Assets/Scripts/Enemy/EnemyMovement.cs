using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private TrackablePlayer player;
    private NavMeshAgent navMeshAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject?.GetComponent<TrackablePlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (player.IsCloaked())
            {
                navMeshAgent.ResetPath(); // Stop moving if the player is cloaked
            }
            else
            {
                navMeshAgent.SetDestination(player.transform.position);
            }
        }
    }

}
