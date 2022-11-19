using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Base class for global grass material profile
    /// </summary>
    public abstract class GrassMaterialProfile : ScriptableObject
    {
        /// <summary>
        /// Sets grass material data to global grass shader variables
        /// </summary>
        public abstract void SetMaterialToGrass();
    }
}
