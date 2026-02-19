using UnityEngine;
using System.Collections;

public class HammerModule : Module
{
    public float hammerDownAngle = 90.0f;
    public float hammerUpAngle = 0.0f;
    public float hammerWindUpAngle = -45.0f;

    public float windUpTime = 0.2f;
    public float swingTime = 0.05f;
    public float recoveryTime = 0.2f;

    public float damage = 50.0f;
    public float range = 2.0f;

    private bool down = false;

    protected override void PerformAction(Vector3 direction)
    {
        StartCoroutine(SwingHammer(direction));
    }

    private IEnumerator SwingHammer(Vector3 direction)
    {
        // Wind up the hammer
        down = true;
        float elapsed = 0.0f;
        while (elapsed < windUpTime)
        {
            float angle = Mathf.Lerp(hammerUpAngle, hammerWindUpAngle, elapsed / windUpTime);
            transform.localRotation = Quaternion.Euler(angle, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Swing down
        elapsed = 0.0f;
        while (elapsed < swingTime)
        {
            float angle = Mathf.Lerp(hammerWindUpAngle, hammerDownAngle, elapsed / swingTime);
            transform.localRotation = Quaternion.Euler(angle, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // TODO: Damage entities hit

        // Recovery
        elapsed = 0.0f;
        while (elapsed < recoveryTime)
        {
            float angle = Mathf.Lerp(hammerDownAngle, hammerUpAngle, elapsed / recoveryTime);
            transform.localRotation = Quaternion.Euler(angle, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        down = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        // disable game object if tag is "Enemy"
        if (other.gameObject.CompareTag("Enemy") && down == true)
        {
            other.gameObject.SetActive(false);
            // EnemyMovement.Instance.hit();
            // Destroy(gameObject);
        }
    }
}
