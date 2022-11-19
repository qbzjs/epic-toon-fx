using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderCombat : MonoBehaviour
{
    Rigidbody colliderRigidbody;
    Beyblade myBeybalede;
 
    private void Start()
    {
        colliderRigidbody=GetComponentInParent<Rigidbody>();
        myBeybalede = GetComponent<Beyblade>();
   
    }

    private void OnCollisionEnter(Collision collision)
    {

        //if (collision.gameObject.CompareTag("Enemy"))
        //{
        //    if (collision.transform.TryGetComponent<AILevel>(out AILevel aILevel))
        //    {
        //        if (myPlayerLevel.startLevel>= aILevel.startLevel)
        //        {
        //            myPlayerLevel.levelUp(aILevel.startLevel);
        //            beybladeAI beyblade = aILevel.GetComponent<beybladeAI>();
        //            if (collision.transform.TransformPoint(transform.position).x>-0.5f)
        //            {
                      
        //                Vector3 dir = new Vector3(Random.Range(5, 8),0 , Random.Range(10,15));
        //                beyblade.deadFlyBeyblade(dir, Random.Range(6, 8)*Vector3.up, collision.contacts[0].point);
        //            }
        //            else
        //            {
        //                beyblade.deadFlyBeyblade(-collision.contacts[0].normal * 15f, Vector3.up *Random.Range(6,8), collision.contacts[0].point);

        //            }
                 
        //        }
        //    }
        //}

    }
}
