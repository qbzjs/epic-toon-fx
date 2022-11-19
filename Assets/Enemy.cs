using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RagdollMecanimMixer;


public class Enemy : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] RamecanMixer mecanimMixer;
    [SerializeField] Rigidbody hip;
    [SerializeField] GameObject hiteffect;
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.CompareTag("Player"))
    //    {
    //        GetComponent<Collider>().enabled = false;
    //        GetComponent<Rigidbody>().isKinematic = false;
    //        mecanimMixer.currentState = 1;
    //        mecanimMixer.ChangeStateImmediately();
    //        animator.SetTrigger("hit");


    //        hip.velocity = Vector3.zero;
    //        Vector3 force = transform.position.x > 0 ? Vector3.right * Random.Range( 5,10f) : -Vector3.right * Random.Range(5, 10f);
    //        force += Vector3.up * Random.Range(5, 10f);
    //        force += Vector3.forward * Random.Range(5, 10f);
    //        hip.AddForce(force*50f, ForceMode.Impulse);
    //    }
       
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
            mecanimMixer.currentState = 1;
            mecanimMixer.ChangeStateImmediately();
            animator.SetTrigger("hit");

            Instantiate(hiteffect, transform.TransformPoint(Vector3.zero + Vector3.up), Quaternion.Euler(0,180,0));
            hip.velocity = Vector3.zero;
            Vector3 force = transform.position.x > 0 ? Vector3.right * Random.Range(1, 3) : -Vector3.right * Random.Range(1, 3);
            force += Vector3.up * Random.Range(5, 10f);
            force += Vector3.forward * Random.Range(5, 10f);
            hip.AddForce(force * 50f, ForceMode.Impulse);
        }
    }
}
