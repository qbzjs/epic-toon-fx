using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrow : MonoBehaviour {

    public GameObject ball;
    public float speed = 1;
    public float lifeTime = 5;

	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1"))
        {
            ThrowBall();
        }
	}

    public void ThrowBall()
    {
        GameObject instance = Instantiate(ball, transform.position, transform.rotation, transform.root.parent);
        instance.GetComponent<Rigidbody>().velocity = instance.transform.forward * speed;
        Destroy(instance, lifeTime);
    }
}
