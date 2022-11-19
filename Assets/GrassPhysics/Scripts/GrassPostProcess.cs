using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Implement this interface and store your post process state
    /// </summary>
    public interface GrassPostProcessState { }

    /// <summary>
    /// Derive from this abstract class to implement your post process effects to texture
    /// </summary>
    [System.Serializable]
    public abstract class GrassPostProcess : ScriptableObject
    {
        public abstract void Initialize(GrassPhysicsArea grassPhysicsArea, out GrassPostProcessState state);
        public abstract void DoPostProcess(GrassPhysicsArea grassPhysicsArea, ref GrassPostProcessState state, ref Texture texture);
    }
}
