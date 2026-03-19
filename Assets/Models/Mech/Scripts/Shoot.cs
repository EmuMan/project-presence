using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Shoot : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public NavMeshAgent agent;
    public Transform body;

    [Header("Laser Fire Points")]
    public Transform leftFirePoint;
    public Transform rightFirePoint;

    [Header("Laser Visuals")]
    public LineRenderer leftLaser;
    public LineRenderer rightLaser;
    public float laserDuration = 0.08f;

    [Header("Combat")]
    public float chaseRange = 30f;
    public float attackRange = 18f;
    public float fireRate = 1.5f;
    public float damage = 10f;
    public float maxLaserDistance = 50f;
    public LayerMask hitMask = ~0;

    [Header("Aiming")]
    public float bodyTurnSpeed = 6f;
    public bool aimBodyOnly = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip laserShotClip;

    private float fireTimer = 0f;
    private bool fireLeftNext = true;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (leftLaser != null)
            leftLaser.enabled = false;

        if (rightLaser != null)
            rightLaser.enabled = false;
    }

    void Update()
    {
        if (target == null)
            return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        //HandleMovement(distanceToTarget);
        AimAtTarget();

        if (distanceToTarget <= attackRange)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= 1f / fireRate)
            {
                fireTimer = 0f;
                FireAlternatingLaser();
            }
        }
    }

/*
    void HandleMovement(float distanceToTarget)
    {
        if (agent == null)
            return;

        if (distanceToTarget > chaseRange)
        {
            agent.isStopped = true;
            return;
        }

        if (distanceToTarget > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }
	*/

    void AimAtTarget()
    {
        Transform aimTransform = aimBodyOnly && body != null ? body : transform;

        Vector3 direction = target.position - aimTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        aimTransform.rotation = Quaternion.Slerp(
            aimTransform.rotation,
            targetRotation,
            Time.deltaTime * bodyTurnSpeed
        );
    }

    void FireAlternatingLaser()
    {
        if (fireLeftNext)
        {
            FireLaser(leftFirePoint, leftLaser);
        }
        else
        {
            FireLaser(rightFirePoint, rightLaser);
        }

        fireLeftNext = !fireLeftNext;
    }

    void FireLaser(Transform firePoint, LineRenderer laser)
    {
        if (firePoint == null)
            return;

        if (audioSource != null && laserShotClip != null)
            audioSource.PlayOneShot(laserShotClip);

        Vector3 start = firePoint.position;
        Vector3 direction = firePoint.forward;
        Vector3 end = start + direction * maxLaserDistance;

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, maxLaserDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
			
            end = hit.point;

            Health playerHealth = hit.collider.GetComponent<Health>();
            if (playerHealth == null)
                playerHealth = hit.collider.GetComponentInParent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
			
        }

        Debug.DrawRay(start, direction * maxLaserDistance, Color.red, 0.15f);

        if (laser != null)
            StartCoroutine(ShowLaser(laser, start, end));
    }

    IEnumerator ShowLaser(LineRenderer laser, Vector3 start, Vector3 end)
    {
        laser.enabled = true;
        laser.positionCount = 2;
        laser.SetPosition(0, start);
        laser.SetPosition(1, end);

        yield return new WaitForSeconds(laserDuration);

        laser.enabled = false;
    }
}




/*
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Shoot : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Weapon")]
    public Transform[] firePoints;
    public LineRenderer[] laserRenderers;
    public float attackRange = 20f;
    public float fireRate = 1f;
    public float laserDuration = 0.08f;
    public float damage = 10f;
    public float maxLaserDistance = 50f;

    [Header("Aim")]
    public Transform body;
    public float turnSpeed = 5f;

    [Header("Optional")]
    public NavMeshAgent agent;
    public LayerMask hitMask;

    private float fireTimer;

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (agent != null)
        {
            if (distance <= attackRange)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                return;
            }
        }

        AimAtTarget();

        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate)
        {
            fireTimer = 0f;
            FireAllLasers();
        }
    }

    void AimAtTarget()
    {
        if (body == null) return;

        Vector3 direction = target.position - body.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }

    void FireAllLasers()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            FireLaser(i);
        }
    }

    void FireLaser(int index)
    {
        if (index >= firePoints.Length) return;
        if (index >= laserRenderers.Length) return;

        Transform firePoint = firePoints[index];
        LineRenderer lr = laserRenderers[index];

        Vector3 start = firePoint.position;
        Vector3 direction = firePoint.forward;

        Vector3 end = start + direction * maxLaserDistance;

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, maxLaserDistance, hitMask))
        {
            end = hit.point;

			
			//edit this
            PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
			
        }

        StartCoroutine(ShowLaser(lr, start, end));
    }

    IEnumerator ShowLaser(LineRenderer lr, Vector3 start, Vector3 end)
    {
        lr.enabled = true;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        yield return new WaitForSeconds(laserDuration);

        lr.enabled = false;
    }
}
*/



/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
*/
