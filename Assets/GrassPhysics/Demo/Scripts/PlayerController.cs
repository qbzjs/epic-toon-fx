
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 12;
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float dashMultiplier = 1.5f;
    [SerializeField] private float jumpStrength = 5f;

    [SerializeField] private bool mobileInput = false;

    [Space]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private new Camera camera;
    [SerializeField] private Transform groundCheck;

    private bool jump = false;
    private float verticalVelocity = 0;
    private float boost = 1f;
    private float x, z;

    private Vector3 move = Vector3.zero;

    private void Start()
    {
        if (!controller) controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponent<Animator>();
        if (!camera) camera = Camera.main;
    }

    public void SetMovement(Vector2 movement)
    {
        x = movement.x;
        z = movement.y;
    }

    private void Update()
    {
        if(!mobileInput)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }


    }

    private void FixedUpdate()
    {
        Movement();
        CharacterAnimation();
    }

    private void LateUpdate()
    {

    }

    private bool IsGrounded()
    {
        return Physics.OverlapSphere(groundCheck.position, 0.1f).Length > 0;
    }

    private void CharacterAnimation()
    {
        animator.SetFloat("Forward", move.magnitude);
    }

    private void Movement()
    {
        Vector3 right = camera.transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);

        if (IsGrounded())
        {
            move = forward * z + right * x;
            if (move != Vector3.zero) move = move.normalized * Mathf.Min(move.magnitude, 1f);
            verticalVelocity = -0.1f;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                boost = dashMultiplier;
            }
            else
            {
                boost = 1;
            }

            if (jump)
            {
                verticalVelocity += jumpStrength;
            }

            Vector3 targetVector = move;
            targetVector.y = 0;
            if (targetVector.magnitude > 0)
            {
                targetVector = targetVector.normalized;

                targetVector.y = 0;
                if (targetVector != Vector3.zero)
                {
                    Quaternion desiredRotation = Quaternion.LookRotation(targetVector);
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        desiredRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * speed * boost * Time.deltaTime);
        jump = false;

        Debug.DrawRay(transform.position, transform.forward, Color.black);
        Debug.DrawRay(transform.position, controller.velocity.normalized, Color.blue);
    }
}