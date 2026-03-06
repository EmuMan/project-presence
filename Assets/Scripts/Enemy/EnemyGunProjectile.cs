using UnityEngine;

public class RandomShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootingPoint;

    public float minShootDelay = 1f;
    public float maxShootDelay = 3f;

    private TrackablePlayer player;

    void Start()
    {
        StartCoroutine(ShootRoutine());

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject?.GetComponent<TrackablePlayer>();
    }

    System.Collections.IEnumerator ShootRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minShootDelay, maxShootDelay);
            yield return new WaitForSeconds(delay);

            if (player != null && !player.IsCloaked())
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        projectile.GetComponent<BasicProjectile>().Initialize(shootingPoint.forward);
    }
}
