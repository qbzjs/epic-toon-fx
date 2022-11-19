using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ShadedTechnology.GrassPhysics;

public class EmptyPostProcessState : GrassPostProcessState { }

[System.Serializable]
[Name("Custom Post Process")]
public class CustomPostProcess : GrassPostProcess
{
    [SerializeField]
    public int A;

    [SerializeField]
    public float B;

    public int C;

    public override void Initialize(GrassPhysicsArea grassPhysicsArea, out GrassPostProcessState state)
    {
        //Initialize your post process here
        state = new EmptyPostProcessState();
    }

    public override void DoPostProcess(GrassPhysicsArea grassPhysicsArea, ref GrassPostProcessState state, ref Texture texture)
    {
        //Process grass depth texture here
    }
}
