using UnityEngine;
using UnityEngine.AI;
public class EnemyMovement : MonoBehaviour
{
    Transform player;
    private NavMeshAgent navMeshAgent;

    public static EnemyMovement Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
       if (player != null)
       {    
           navMeshAgent.SetDestination(player.position);
       }        
    }

    public void hit()
    {
        Destroy(gameObject);
    }
}
