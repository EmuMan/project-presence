using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class EnemyManager : MonoBehaviour
{
    public Vector2 xRange = new Vector2(-23f, 23f);
    public Vector2 yRange = new Vector2(-23f, 23f);
    public GameObject prefab;
    public float spawnDelay = 4f;

    IEnumerator Start()
    {
        while (true)
        {
            Vector3 pos1 = new Vector3(-23, 2f, Random.Range(yRange.x, yRange.y));
            Instantiate(prefab, pos1, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);

            Vector3 pos2 = new Vector3(23, 2f, Random.Range(yRange.x, yRange.y));
            Instantiate(prefab, pos2, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);


            Vector3 pos3 = new Vector3(Random.Range(xRange.x, xRange.y), 2f, 23);
            Instantiate(prefab, pos3, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);

            Vector3 pos4 = new Vector3(Random.Range(xRange.x, xRange.y), 2f, -23);
            Instantiate(prefab, pos4, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);

        }
    }
}
