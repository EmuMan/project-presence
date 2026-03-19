using UnityEngine;
using UnityEngine.AI;

public class MechFollowNavMesh : MonoBehaviour
{
    private Enemy mechEnemy;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        mechEnemy = GetComponent<Enemy>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;

        Debug.Log("Enemy found: " + (mechEnemy != null));
        Debug.Log("NavMeshAgent found: " + (navMeshAgent != null));
    }

    void Update()
    {
        Transform target = mechEnemy.GetTarget();
        if (mechEnemy.canAct && target != null)
        {
            navMeshAgent.SetDestination(target.position);
            // Direction to target
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f; // prevent tilting

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Smooth rotation
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * 360f // increase for faster turning
                );
            }
        }
        else
        {
            navMeshAgent.ResetPath();
        }
    }
}
/*
using UnityEngine;

public class MechFollowMovement : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stopDistance = 2f;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            direction.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}
*/

/*
using UnityEngine;

public class MechFollowPlayer : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stopDistance = 2f;

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}
*/

/*
using UnityEngine;

public class MechFollowMovement : MonoBehaviour
{
    private Transform player;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stopDistance = 2f;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}
*/
