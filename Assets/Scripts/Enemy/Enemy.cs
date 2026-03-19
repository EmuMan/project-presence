using UnityEngine;

public class Enemy : MonoBehaviour
{
    private TrackablePlayer player;

    public bool canAct = true;
    private float disabledTimer;

    [SerializeField] private bool isSleeping = true;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject?.GetComponent<TrackablePlayer>();
    }

    void FixedUpdate()
    {
        if (!canAct)
        {
            disabledTimer -= Time.fixedDeltaTime;
            if (disabledTimer <= 0f)
            {
                canAct = true;
                disabledTimer = 0f;
            }
        }
    }

    public void DisableForSeconds(float duration)
    {
        canAct = false;
        disabledTimer = duration;
    }

    public Transform GetTarget()
    {
        if (isSleeping) return null;
        if (player != null)
        {
            return player.IsCloaked() ? null : player.transform;
        }
        return null;
    }

    public void Activate()
    {
        isSleeping = false;
    }

    public void Deactivate()
    {
        isSleeping = true;
    }
}
