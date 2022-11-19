using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Manages grass trail effect post process using fragment shader.
    /// </summary>
    public class FragmentShaderTrailEffect
    {
        private Vector2Int textureSize = new Vector2Int(1024, 1024);
        
        private RenderTexture PostTex
        {
            get
            {
                if (!_postTex)
                {
                    _postTex = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.RFloat);
                    _postTex.wrapMode = TextureWrapMode.Clamp;
                    _postTex.anisoLevel = 0;
                }
                return _postTex;
            }
            set
            {
                _postTex = value;
            }
        }
        private RenderTexture _postTex;
        private Material material;

        public FragmentShaderTrailEffect(Vector2Int textureSize, Shader shader)
        {
            this.textureSize = textureSize;
            material = new Material(shader);
        }

        /// <summary>
        /// Process camera target texture and sets result texture to global grass shader using <c>GrassFullPhysics</c> function
        /// </summary>
        /// <param name="cam">Camera object</param>
        /// <param name="recoverySpeed">Speed of grass recovery</param>
        /// <param name="physicsScript">Grass physics script</param>
        public void DoPostProcess(ref Texture targetTexture, Vector3 areaSize, Vector3 movement, float recoverySpeed)
        {
            movement = -new Vector3(movement.x / areaSize.x,
                                    movement.y / areaSize.y,
                                    movement.z / areaSize.z);
            RenderTexture temp = RenderTexture.GetTemporary(textureSize.x, textureSize.y, 0, RenderTextureFormat.RFloat);
            material.SetVector("_Movement", movement);
            material.SetTexture("_PrevTex", PostTex);
            material.SetFloat("_RecoverySpeed", (Time.deltaTime * recoverySpeed) / areaSize.y);
            Graphics.Blit(targetTexture, temp, material);
            Graphics.Blit(temp, PostTex);
            RenderTexture.ReleaseTemporary(temp);

            targetTexture = PostTex;
        }
    }
}
