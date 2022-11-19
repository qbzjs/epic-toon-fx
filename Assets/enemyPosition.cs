using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyPosition : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    public Vector3 startPos;
    public float xDistance;
    public float zDistance;
    public int enemyrowCount;
    public float scaleFactor = 0.1f;

    private void Start()
    {
        Vector3 firstPos = startPos;
        Vector3 firstScale = Vector3.one;
        for (int i = 0; i < enemyrowCount; i++)
        {
            Vector3 pos = firstPos;
            Vector3 posright = pos + Vector3.right * xDistance;
            Vector3 leftright = pos - Vector3.right * xDistance;

            GameObject enemy1 = Instantiate(enemyPrefab, posright, Quaternion.Euler(0, 180, 0));


            enemy1.transform.localScale = firstScale;
            GameObject enemy2 = Instantiate(enemyPrefab, leftright, Quaternion.Euler(0, 180, 0));



            enemy2.transform.localScale = firstScale;
            firstPos += Vector3.forward * zDistance;

            firstScale *= scaleFactor;
          //  xDistance *= scaleFactor;
            zDistance *= scaleFactor;


        }
    }

}
