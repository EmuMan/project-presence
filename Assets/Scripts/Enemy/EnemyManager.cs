using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class EnemyManager : MonoBehaviour
{
    //public Vector2 xRange = new Vector2(-23f, 23f);
    //public Vector2 yRange = new Vector2(-23f, 23f);
    public GameObject prefab;
    public float spawnDelay = 4f;   //can be adjucted in the inspector
    public int spawnQuantity = 5;   //update in the inpsector 
    public bool spawnBool = true;

    public Vector2 spawnPoint1 = new Vector2(0f, 0f);   //update this in the inspector
    public Vector2 spawnPoint2 = new Vector2(0f, 0f);   //update this in the inspector


    IEnumerator Start()
    {
        while (spawnBool == true)
        {
            /*
            //This is the code to spawn the enemies at the perimeter (adjust depending on the size of the ground)
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
            */

            //this is the code to spawn enemies at specific points ()
            Vector3 pos1 = new Vector3(spawnPoint1.x, 2f, spawnPoint1.y);
            Instantiate(prefab, pos1, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);

            spawnQuantity -= 1;

            if (spawnQuantity <= 0)
            {
                spawnBool = false;
            }

            else
            {

                Vector3 pos2 = new Vector3(spawnPoint2.x, 2f, spawnPoint2.y);
                Instantiate(prefab, pos2, Quaternion.identity);
                yield return new WaitForSeconds(spawnDelay);

                spawnQuantity -= 1;

                if (spawnQuantity <= 0)
                {
                    spawnBool = false;
                }
            }

        }
    }
}
