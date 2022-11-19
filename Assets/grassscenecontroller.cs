using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class grassscenecontroller : MonoBehaviour
{
    [SerializeField] Beyblade beyblade;
    [SerializeField] FloatingJoystick floatingJoystick;

    private void Update()
    {
        beyblade.setPlayerInput(floatingJoystick.Horizontal,floatingJoystick.Vertical);
       
    }

}
