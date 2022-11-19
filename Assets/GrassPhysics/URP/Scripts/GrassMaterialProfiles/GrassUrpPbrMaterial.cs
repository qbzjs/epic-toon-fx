using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class stores specular grass material settings
    /// </summary>
    [CreateAssetMenu(fileName = "GrassUrpPbrMaterial", menuName = "GrassPhysics/GrassMaterialProfile/GrassUrpPbrMaterial")]
    [GrassMaterial(name = "URP PBR Material", fileName = "GrassUrpPbrMaterial", fragmentShader = "../../URP/Shaders/GrassSurfaces/GrassUrpPbrSurface.cginc")]
    public class GrassUrpPbrMaterial : GrassMaterialProfile
    {
        [LowEndPlatformWarning("Some platforms doesn't support PBR materials, so if you getting bad results try changing Grass Material.")]
        public Color grassTint = Color.white;
        [Range(-0.6f, 0.6f)]
        public float glossThreshHeight = -0.3f;
        [Range(0f, 10f)]
        public float glossThreshSmoothness = 2f;
        [Range(0f, 1f)]
        public float smoothness;
        [Range(0f, 1f)]
        public float metallic;
        public Color emission = Color.black;

        /// <summary>
        /// Sets grass material data to global grass shader variables
        /// </summary>
        public override void SetMaterialToGrass()
        {
            Shader.SetGlobalColor("_GrassColorTint", grassTint);
            Shader.SetGlobalFloat("_GrassSpecularTresholdHeight", glossThreshHeight);
            Shader.SetGlobalFloat("_GrassSpecularTresholdSmoothness", glossThreshSmoothness);
            Shader.SetGlobalFloat("_GrassSmoothness", smoothness);
            Shader.SetGlobalFloat("_GrassMetallic", metallic);
            Shader.SetGlobalColor("_GrassEmission", emission);
        }
    }
}
