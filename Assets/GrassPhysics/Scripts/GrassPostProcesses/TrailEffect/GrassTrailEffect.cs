using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    public class GrassTrailEffectState : GrassPostProcessState
    {
        private FragmentShaderTrailEffect postProcess;

        public GrassTrailEffectState(GrassPhysicsArea grassPhysicsArea, Shader postProcessShader)
        {
            postProcess = new FragmentShaderTrailEffect(grassPhysicsArea.textureSize, postProcessShader);
        }

        public void DoPostProcess(GrassPhysicsArea grassPhysicsArea, ref Texture texture, float recoverySpeed)
        {
            postProcess.DoPostProcess(ref texture,
                                      grassPhysicsArea.areaSize,
                                      grassPhysicsArea.GetLastMovementVector(),
                                      recoverySpeed);
        }
    }

    [System.Serializable]
    [Name("Grass Trail Effect (Standard)", "Trail effect using FragmentShader")]
    public class GrassTrailEffect : GrassPostProcess
    {
        [SerializeField]
        [Range(0,1)]
        public float recoverySpeed = 0.1f;

        [HideInInspector]
        [SerializeField]
        private Shader postProcessShader;

        private void Awake()
        {
            if(null == postProcessShader)
            {
                postProcessShader = Shader.Find("Hidden/GrassPhysics/GrassTrailEffect");
            }
        }

        public override void Initialize(GrassPhysicsArea grassPhysicsArea, out GrassPostProcessState state)
        {
            state = new GrassTrailEffectState(grassPhysicsArea, postProcessShader);
        }

        public override void DoPostProcess(GrassPhysicsArea grassPhysicsArea, ref GrassPostProcessState state, ref Texture texture)
        {
            (state as GrassTrailEffectState).DoPostProcess(grassPhysicsArea, ref texture, recoverySpeed);
        }

    }
}
