using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Grass physics modes
    /// </summary>
    public enum GrassPhysicsMode { None = 0, Simple = 1, Full = 2 };

    /// <summary>
    /// Base class for grass physics scripts
    /// </summary>
    [RequireComponent(typeof(GrassManager))]
    public abstract class GrassPhysics : MonoBehaviour
    {
    }
}
