using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class EnemyManager : MonoBehaviour
{
    public Vector2 xRange = new Vector2(-23f, 23f);
    public Vector2 yRange = new Vector2(-23f, 23f);
    public GameObject prefab;
    public float spawnDelay = 4f;

    public int spawnQuantity = 12;
    public bool spawnBool = true;


    //Spawns enemy game objects along at the corners
    IEnumerator Start()
    {
        while (spawnBool)
        {
            /*
            Vector3 pos1 = new Vector3(-23, 2f, 23);
            Instantiate(prefab, pos1, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);

            Vector3 pos2 = new Vector3(23, 2f, -23);
            Instantiate(prefab, pos2, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);

            Vector3 pos3 = new Vector3(23, 2f, 23);
            Instantiate(prefab, pos3, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);

            Vector3 pos4 = new Vector3(-23, 2f, -23);
            Instantiate(prefab, pos4, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
            */

            Vector3 pos1 = new Vector3(23, 2f, 0);
            Instantiate(prefab, pos1, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);

            
            Vector3 pos2 = new Vector3(-23, 2f, 0);
            Instantiate(prefab, pos2, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);

            spawnQuantity -= 2; //4 new spawns

            if (spawnQuantity <= 0)
            {
                spawnBool = false;
            }

        }
    }


}
