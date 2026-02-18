using UnityEngine;

public class RandomShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootingPoint;

    public float minShootDelay = 1f;
    public float maxShootDelay = 3f;

    void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    System.Collections.IEnumerator ShootRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minShootDelay, maxShootDelay);
            yield return new WaitForSeconds(delay);

            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
    }
}