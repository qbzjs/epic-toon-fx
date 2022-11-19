using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Manages grass trail effect post process using compute shader.
    /// </summary>
    public class ComputeShaderTrailEffect
    {
        public ComputeShader deformShader;

        private Vector2Int textureSize = new Vector2Int(1024, 1024);
        private int textureIndex;
        private int deformShaderID;
        private RenderTexture[] deformTextures = new RenderTexture[2];
        private Texture DeformBuffer
        {
            get
            {
                if (!deformTextures[textureIndex % 2])
                {
                    deformTextures[textureIndex % 2] = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.RFloat)
                    {
                        filterMode = FilterMode.Trilinear,
                        wrapMode = TextureWrapMode.Clamp,
                        anisoLevel = 0
                    };
                }
                return deformTextures[textureIndex % 2];
            }
            set
            {
                deformTextures[textureIndex % 2] = value as RenderTexture;
                deformTextures[textureIndex % 2].filterMode = FilterMode.Trilinear;
                deformTextures[textureIndex % 2].wrapMode = TextureWrapMode.Clamp;
                deformTextures[textureIndex % 2].anisoLevel = 0;
            }
        }
        private Texture DeformTexture
        {
            get
            {
                if (!deformTextures[(textureIndex + 1) % 2])
                {
                    deformTextures[(textureIndex + 1) % 2] = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.RFloat)
                    {
                        filterMode = FilterMode.Trilinear,
                        wrapMode = TextureWrapMode.Clamp,
                        anisoLevel = 0
                    };
                }
                return deformTextures[(textureIndex + 1) % 2];
            }
            set
            {
                deformTextures[(textureIndex + 1) % 2] = value as RenderTexture;
                deformTextures[(textureIndex + 1) % 2].filterMode = FilterMode.Trilinear;
                deformTextures[(textureIndex + 1) % 2].wrapMode = TextureWrapMode.Clamp;
                deformTextures[(textureIndex + 1) % 2].anisoLevel = 0;
            }
        }

        private void CheckForDeformTexturesAndCreateIfNull()
        {
            if (deformTextures.Length != 2 || !deformTextures[0] || !deformTextures[1])
            {
                deformTextures = new RenderTexture[2];
                for (int i = 0; i < deformTextures.Length; ++i)
                {
                    deformTextures[i] = new RenderTexture(textureSize.x, textureSize.y, 0, RenderTextureFormat.RFloat)
                    {
                        filterMode = FilterMode.Trilinear,
                        wrapMode = TextureWrapMode.Clamp,
                        anisoLevel = 0
                    };
                }
            }
        }
        
        public ComputeShaderTrailEffect(ComputeShader computeShader, Texture targetTexture, Vector2Int textureSize)
        {
            this.deformShader = computeShader;
            this.textureSize = textureSize;

            deformShaderID = deformShader.FindKernel("GrassTrailUpdate");
            CheckForDeformTexturesAndCreateIfNull();
            for (int i = 0; i < deformTextures.Length; ++i)
            {
                deformTextures[i].enableRandomWrite = true;
                deformTextures[i].Create();
            }
        }
        
        public void DoPostProcess(ref Texture texture, Vector3 movement, float stepSize, float recoverySpeed, float farClipPlane)
        {
            deformShader.SetTexture(deformShaderID, "Input", texture);
            movement = -new Vector3(MathHelper.FloorDivide(movement.x, stepSize) * GlobalConstants.STEP_PX,
                                   movement.y / farClipPlane,
                                   MathHelper.FloorDivide(movement.z, stepSize) * GlobalConstants.STEP_PX);
            deformShader.SetVector("Movement", movement);
            deformShader.SetFloat("RecoverySpeed", (Time.deltaTime * recoverySpeed) / farClipPlane);
            deformShader.SetTexture(deformShaderID, "PreviousState", DeformBuffer);
            deformShader.SetTexture(deformShaderID, "Result", DeformTexture);
            deformShader.Dispatch(deformShaderID, textureSize.x / 8, textureSize.y / 8, 1);

            textureIndex = (textureIndex + 1) % 2;

            texture = deformTextures[textureIndex];
        }
    }
}
