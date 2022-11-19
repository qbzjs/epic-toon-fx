using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ShadedTechnology.GrassPhysics
{
    public class GrassTrailEffectComputeState : GrassPostProcessState
    {
        private ComputeShaderTrailEffect postProcess;

        public GrassTrailEffectComputeState(GrassPhysicsArea grassPhysicsArea, ComputeShader computeShader)
        {
            postProcess = new ComputeShaderTrailEffect(computeShader, grassPhysicsArea.depthTexture, grassPhysicsArea.textureSize);
        }

        public void DoPostProcess(GrassPhysicsArea grassPhysicsArea, ref Texture texture, float recoverySpeed)
        {
            postProcess.DoPostProcess(ref texture,
                                      grassPhysicsArea.GetLastMovementVector(),
                                      grassPhysicsArea.Step_size, recoverySpeed,
                                      grassPhysicsArea.areaSize.y);
        }
    }

    [System.Serializable]
    [Name("Grass Trail Effect (ComputeShader)", "Trail effect using ComputeShader (requires DirectX 11) sometimes can be a little bit faster than standard")]
    public class GrassTrailEffectCompute : GrassPostProcess
    {
        [Range(0, 1)]
        public float recoverySpeed = 0.1f;

        [HideInInspector]
        [SerializeField]
        private ComputeShader computeShader;

        public override void Initialize(GrassPhysicsArea grassPhysicsArea, out GrassPostProcessState state)
        {
            state = new GrassTrailEffectComputeState(grassPhysicsArea, computeShader);
        }

        public override void DoPostProcess(GrassPhysicsArea grassPhysicsArea, ref GrassPostProcessState state, ref Texture texture)
        {
            (state as GrassTrailEffectComputeState).DoPostProcess(grassPhysicsArea, ref texture, recoverySpeed);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Returns compute shader used for grass trail effect
        /// </summary>
        /// <returns>Compute shader for grass trail effect</returns>
        public static ComputeShader GetTrailEffectComputeShader()
        {
            string path = AssetsManager.GetGrassAssetPath() + "/Shaders/PostProcesses/GrassTrailEffect.compute";
            return AssetDatabase.LoadAssetAtPath<ComputeShader>(path);
        }

        private void Awake()
        {
            if(null == computeShader)
            {
                computeShader = GetTrailEffectComputeShader();
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }
}
