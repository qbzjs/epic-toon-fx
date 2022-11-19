using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    //Beyblade beyblade;
    //Rigidbody rb;

    //[SerializeField] float ForwardSpeed;
    //[SerializeField] float minX;
    //[SerializeField] float maxX;
    //[SerializeField] float SpeedBoostPosX;
    //[SerializeField] float ForawrdBoostSpeedMultiper;
    //PlayerZoomFovEffect playerZoom;


    //[SerializeField] float rotSpeed = 2f;
    //Transform targetObject;

    //private void Start()
    //{
    //    beyblade = GetComponent<Beyblade>();
    //    rb = GetComponent<Rigidbody>();
    //    playerZoom = GetComponent<PlayerZoomFovEffect>();
    //    transform.position = new Vector3(minX, transform.position.y, transform.position.z);

    //    targetObject = new GameObject("TargetRot").transform;
    //    targetObject.transform.position = transform.position + Vector3.forward * 5f;
    //}
    // float a = 0;

    //float targetXT = 0;
    //private void Update()
    //{



    //    Vector3 setSpeed = Vector3.zero;


    //    if (transform.position.x < SpeedBoostPosX)
    //    {
    //        setSpeed.z = ForawrdBoostSpeedMultiper * ForwardSpeed;
    //        a += Time.deltaTime * 3f;
    //    }
    //    else
    //    {

    //        setSpeed.z = ForwardSpeed;
    //        a -= Time.deltaTime * 2f;
    //    }


    //    a = Mathf.Clamp((float)a, 0f, 1f);

    //    if (playerZoom != null)
    //    {
    //        playerZoom.setFov(a);
    //    }

    //    float targetX = 0;

    //    if (Input.GetMouseButton(0))
    //    {

    //        //   targetXT += Time.deltaTime * 5f;

    //        targetXT = 1;
    //    }
    //    else
    //    {
    //        // targetXT -= Time.deltaTime * 5f;
    //        targetXT = 0;

    //    }
    //    targetXT = Mathf.Clamp01(targetXT);
    //    targetX = Mathf.Lerp(minX, maxX, targetXT);



    //    targetObject.transform.position = new Vector3(targetX, 0f, transform.position.z) + Vector3.forward * 10f;


    //    target = Vector3.Slerp(target, targetObject.position - new Vector3(transform.position.x, 0f, transform.position.z), Time.deltaTime* rotSpeed);


    //    target.Normalize();



    //    // beyblade.setPlayerInputEndlessRunner(1, 1, setSpeed.z,target.x * RightSpeed);
    //    beyblade.denemeSetPlayer(target * setSpeed.z);
      

    //}
    // Vector3 target;
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("End"))
    //    {
    //        StaticEventHandler.CallNextGroundEvent();
    //    }
    //}
}
