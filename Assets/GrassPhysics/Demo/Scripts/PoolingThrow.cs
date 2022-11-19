using System.Collections;
using System.Collections.Generic;
using ShadedTechnology.GrassPhysics;
using UnityEngine;

public class PoolingThrow : MonoBehaviour {
    
    public float speed = 1;
    public float radius = 1;
    public GameObject[] balls;

    private int currentBall = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ThrowBall();
        }
    }

    public void ThrowBall()
    {
        currentBall = currentBall % balls.Length;
        balls[currentBall].transform.position = transform.position;
        balls[currentBall].transform.rotation = transform.rotation;
        balls[currentBall].GetComponent<GrassActor>().radius = radius;
        balls[currentBall].SetActive(true);
        balls[currentBall].GetComponent<Rigidbody>().velocity = balls[currentBall].transform.forward * speed;
        currentBall++;
    }
}
