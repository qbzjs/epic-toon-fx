using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public ThirdPersonCamera.CameraBase cameraBase;
    public PlayerController playerController;

    private Vector2 rotation;

    public void LeftJoysticHandle(Vector2 movement)
    {
        playerController.SetMovement(movement);
    }

    private void Update()
    {
        cameraBase.SetRotation(rotation);
    }

    public void RightJoystickHandle(Vector2 rotation)
    {
        this.rotation = rotation;
    }
}
