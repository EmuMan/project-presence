using UnityEngine;

public class LaserEnemy : MonoBehaviour
{
    public Transform laserOrigin;
    public float gunRange = 15f;
    public float laserDuration = 0.03f;
    public float laserDelay = 1f;
    LineRenderer laserLine;
    public float lastLaserShot = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > (lastLaserShot + laserDelay))
        {
            laserLine.SetPosition(0, laserOrigin.position);
        }
        
    }
}
