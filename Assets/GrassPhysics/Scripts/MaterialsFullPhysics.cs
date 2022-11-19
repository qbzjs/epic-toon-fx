using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class that manages custom grass materials physics in Full Physics Mode
    /// </summary>
    [AddComponentMenu("Grass Physics/For Custom Materials/MeshesFullPhysics")]
    public class MaterialsFullPhysics : MonoBehaviour
    {
        [Range(0,1)]
        public float enlargement = 0.4f;
        public float yImpactOffset = 0f;
        public GrassPhysicsArea physicsArea;

        [Space]
        public Material[] materials;

        private void Start()
        {
            SetPhysicsAreaSettings(physicsArea.areaSize);
        }

        private void FixedUpdate()
        {
            Texture tex;
            physicsArea.GetDepthTexture(out tex);
            UpdateDepthTexture(tex, physicsArea.transform.position);
        }

        /// <summary>
        /// Sets texture to materials
        /// </summary>
        /// <param name="texture">Depth texture to set</param>
        /// <param name="position">Position of texture in world space</param>
        public void UpdateDepthTexture(Texture texture, Vector3 position)
        {
            foreach(Material material in materials)
            {
                material.SetTexture("_GrassDepthTex", texture);
                material.SetVector("_GrassPhysicsAreaPos", position);
            }
        }

        /// <summary>
        /// Sets <see cref="GrassPhysicsArea"/> settings for materials
        /// </summary>
        /// <param name="camSettings">Camera settings to set</param>
        public void SetPhysicsAreaSettings(Vector3 areaSize)
        {
            foreach(Material material in materials)
            {
                material.SetFloat("_GrassPhysicsOffset", yImpactOffset);
                material.SetFloat("_GrassTexEnlargement", enlargement);
                material.SetVector("_GrassPhysicsAreaSize", areaSize);
            }
        }

    }
}
