using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class stores simple grass material settings
    /// </summary>
    [CreateAssetMenu(fileName = "GrassLambertianMaterial", menuName = "GrassPhysics/GrassMaterialProfile/GrassLambertianMaterial (Simple)")]
    [GrassMaterial(name = "Lambertian Material (Simple)", fileName = "GrassLambertianMaterial", surf = "Lambert", fragmentShader = "../LegacyRP/GrassSurfaces/GrassLambertianSurface.cginc")]
    public class GrassLambertianMaterial : GrassMaterialProfile
    {
        public Color grassTint = Color.white;

        /// <summary>
        /// Sets grass material data to global grass shader variables
        /// </summary>
        public override void SetMaterialToGrass()
        {
            Shader.SetGlobalColor("_GrassColorTint", grassTint);
        }
    }
}
