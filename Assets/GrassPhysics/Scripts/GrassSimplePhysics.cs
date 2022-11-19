using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class that manages grass physics in Simple Physics Mode
    /// </summary>
    [AddComponentMenu("Grass Physics/GrassSimplePhysics")]
    public class GrassSimplePhysics : GrassPhysics
    {
        public bool canTilt = true;
        
        [Tooltip("Values less or equal to zero means no limit")]
        public OptionalFloat verticalDisplacementLimit;
        [Tooltip("Values less or equal to zero means no limit")]
        public OptionalFloat horizontalDisplacementLimit;

        public LimitedSizeArray_GrassActor grassActors;
        private Vector4[] bufferData = new Vector4[GlobalConstants.MAX_GRASS_ACTORS];

        public void UpdateGrassActorsCount()
        {
            Shader.SetGlobalInt("_TargetsCount", grassActors.Length);
        }

        private void Start()
        {
            Shader.SetGlobalInt("_GrassPhysicsMode", (int)GrassPhysicsMode.Simple);
            Shader.SetGlobalVector("_DisplacementLimits", new Vector4(0, verticalDisplacementLimit.Value, horizontalDisplacementLimit.Value, 0));
            Shader.SetGlobalInt("_CanTilt", canTilt ? 1 : 0);
            UpdateGrassActorsCount();
            grassActors.onArraySizeChange += UpdateGrassActorsCount;

#if UNITY_EDITOR
            verticalDisplacementLimit.onValueChange = () => 
            Shader.SetGlobalVector("_DisplacementLimits", new Vector4(0, verticalDisplacementLimit.Value,
            horizontalDisplacementLimit.Value, 0));
            horizontalDisplacementLimit.onValueChange = () => 
            Shader.SetGlobalVector("_DisplacementLimits", new Vector4(0, verticalDisplacementLimit.Value,
            horizontalDisplacementLimit.Value, 0));
#endif
        }

        public void AddGrassActor(GrassActor grassActor)
        {
            grassActors.AddTargetToArray(grassActor);
            UpdateGrassActorsCount();
        }

        public void RemoveGrassActor(GrassActor grassActor)
        {
            grassActors.RemoveTargetFromArray(grassActor);
            UpdateGrassActorsCount();
        }

        public void RemoveGrassActorAtIndex(int index)
        {
            grassActors.RemoveTargetAtIndex(index);
            UpdateGrassActorsCount();
        }

        public void SetCanTilt(bool newCanTilt)
        {
            canTilt = newCanTilt;
            Shader.SetGlobalInt("_CanTilt", canTilt ? 1 : 0);
        }

        public void SetVerticalDisplacementLimit(float value, bool enableLimit = true)
        {
            verticalDisplacementLimit.enabled = enableLimit;
            verticalDisplacementLimit.Value = value;
            Shader.SetGlobalVector("_DisplacementLimits", new Vector4(0, verticalDisplacementLimit.Value, horizontalDisplacementLimit.Value, 0));
        }
        
        public void SetHorizontalDisplacementLimit(float value, bool enableLimit = true)
        {
            horizontalDisplacementLimit.enabled = enableLimit;
            horizontalDisplacementLimit.Value = value;
            Shader.SetGlobalVector("_DisplacementLimits", new Vector4(0, verticalDisplacementLimit.Value, horizontalDisplacementLimit.Value, 0));
        }

        private void FixedUpdate()
        {
            for(int i = 0; i < grassActors.Length; ++i)
            {
                if(null == grassActors[i])
                {
                    bufferData[i] = Vector4.zero;
                    continue;
                }
                bufferData[i] = grassActors[i].GetVector4();
            }
            Shader.SetGlobalVectorArray("_TargetsPos", bufferData);
        }
    }
}
