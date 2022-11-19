using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class stores specular grass material settings
    /// </summary>
    [CreateAssetMenu(fileName = "GrassDebugWindMaterial", menuName = "GrassPhysics/GrassMaterialProfile/GrassDebugWindMaterial")]
    [GrassMaterial(name = "Debug Wind Material", fileName = "GrassDebugWindMaterial", surf = "Lambert", fragmentShader = "../LegacyRP/GrassSurfaces/GrassDebugWindSurface.cginc")]
    public class GrassDebugWindMaterial : GrassMaterialProfile
    {
        /// <summary>
        /// Sets grass material data to global grass shader variables
        /// </summary>
        public override void SetMaterialToGrass()
        {
        }
    }
}
