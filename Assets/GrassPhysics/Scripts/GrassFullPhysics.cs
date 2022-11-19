using UnityEngine;
using UnityEngine.Events;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class that manages grass physics in full physics mode: <see cref="GrassPhysicsMode.Full"/>
    /// </summary>
    [AddComponentMenu("Grass Physics/GrassFullPhysics")]
    public class GrassFullPhysics : GrassPhysics
    {
        private Texture camTex;
        
        [Range(0,1)]
        public float enlargement = 0.4f;
        public float grassDepthOffset = 0f;

        public GrassPhysicsArea physicsArea;

        private void Start()
        {
            Shader.SetGlobalInt("_GrassPhysicsMode", (int)GrassPhysicsMode.Full);
            SetPhysicsAreaSettings(physicsArea.areaSize, grassDepthOffset, enlargement);
        }

        private void FixedUpdate()
        {
            physicsArea.GetDepthTexture(out camTex);
            UpdateDepthTexture(camTex, physicsArea.transform.position);
        }

        /// <summary>
        /// Sets texture to global grass shader
        /// </summary>
        /// <param name="texture">Depth texture to set</param>
        /// <param name="position">Position of texture in world space</param>
        public static void UpdateDepthTexture(Texture texture, Vector3 position)
        {
            Shader.SetGlobalTexture("_GrassDepthTex", texture);
            Shader.SetGlobalVector("_GrassPhysicsAreaPos", position);
        }

        /// <summary>
        /// Sets <see cref="GrassPhysicsArea"/> settings for global grass shader
        /// </summary>
        /// <param name="camSettings">Camera settings to set</param>
        public static void SetPhysicsAreaSettings(Vector3 areaSize, float grassDepthOffset, float enlargement)
        {
            Shader.SetGlobalFloat("_GrassPhysicsOffset", grassDepthOffset);
            Shader.SetGlobalFloat("_GrassTexEnlargement", enlargement);
            Shader.SetGlobalVector("_GrassPhysicsAreaSize", areaSize);
        }

    }
}
