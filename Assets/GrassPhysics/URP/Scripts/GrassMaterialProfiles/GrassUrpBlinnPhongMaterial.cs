using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class stores specular grass material settings
    /// </summary>
    [CreateAssetMenu(fileName = "GrassUrpBlinnPhongMaterial", menuName = "GrassPhysics/GrassMaterialProfile/GrassUrpBlinnPhongMaterial")]
    [GrassMaterial(name = "URP BlinnPhong Material (Simple)", fileName = "GrassUrpBlinnPhongMaterial", fragmentShader = "../../URP/Shaders/GrassSurfaces/GrassUrpBlinnPhongSurface.cginc")]
    public class GrassUrpBlinnPhongMaterial : GrassMaterialProfile
    {
        public Color grassTint = Color.white;
        public Color emission = Color.black;

        /// <summary>
        /// Sets grass material data to global grass shader variables
        /// </summary>
        public override void SetMaterialToGrass()
        {
            Shader.SetGlobalColor("_GrassColorTint", grassTint);
            Shader.SetGlobalColor("_GrassEmission", emission);
        }
    }
}
