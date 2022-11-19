using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class stores specular grass material settings
    /// </summary>
    [CreateAssetMenu(fileName = "GrassSpecularMaterial", menuName = "GrassPhysics/GrassMaterialProfile/GrassSpecularMaterial")]
    [GrassMaterial(name = "Specular Material", fileName = "GrassSpecularMaterial", surf = "StandardSpecular", fragmentShader = "../LegacyRP/GrassSurfaces/GrassSpecularSurface.cginc")]
    public class GrassSpecularMaterial : GrassMaterialProfile
    {
        [LowEndPlatformWarning("Some platforms doesn't support specular materials, so if you getting bad results try changing Grass Material.")]
        public Color grassTint = Color.white;
        [Range(-0.6f, 0.6f)]
        public float glossThreshHeight = -0.3f;
        [Range(0f, 10f)]
        public float glossThreshSmoothness = 2f;
        [Range(0f, 1f)]
        public float glossStren;
        public Color specColor = Color.white;
        [Range(0f, 1f)]
        public float specStren;

        /// <summary>
        /// Sets grass material data to global grass shader variables
        /// </summary>
        public override void SetMaterialToGrass()
        {
            Shader.SetGlobalColor("_GrassColorTint", grassTint);
            Shader.SetGlobalFloat("_GrassSpecularTresholdHeight", glossThreshHeight);
            Shader.SetGlobalFloat("_GrassSpecularTresholdSmoothness", glossThreshSmoothness);
            Shader.SetGlobalFloat("_GrassGlossStren", glossStren);
            Shader.SetGlobalColor("_GrassSpecColor", specColor);
            Shader.SetGlobalFloat("_GrassSpecStren", specStren);
        }
    }
}
