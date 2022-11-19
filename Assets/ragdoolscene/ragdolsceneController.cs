using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ragdolsceneController : MonoBehaviour
{
   public Beyblade beyblade;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            beyblade.setPlayerInput(0, 1);
        }
        else
        {
            beyblade.setPlayerInput(0, 0);
        }
    }
}
