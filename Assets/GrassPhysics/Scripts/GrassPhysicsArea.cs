using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Area with depth texture which defines how grass should deform in Full Physics Mode
    /// </summary>
    [AddComponentMenu("Grass Physics/GrassPhysicsArea")]
    public class GrassPhysicsArea : MonoBehaviour
    {
        public Texture depthTexture;

        public Vector3 areaSize = new Vector3(40, 40, 40);
        public Vector2Int textureSize = new Vector2Int(1024, 1024);

        public bool followPlayer = true;
        public Transform target;
        public Vector3 areaOffset = new Vector3(0, -20, 0);

        public GrassPostProcessProfile postProcessProfile;

        private List<GrassPostProcessState> postProcessStates;
        
        [SerializeField]
        public UnityEvent onSetDepthTextureEvent;

        private float lastUpdateTime = 0f;

        private Texture postProcessTexture;

        /// <summary>
        /// Area movement step size in units
        /// </summary>
        public float Step_size
        {
            get
            {
                if (_step_size <= 0)
                {
                    _step_size = (GlobalConstants.STEP_PX * areaSize.z) / textureSize.y;
                }
                return _step_size;
            }
        }
        private float _step_size = -1;

        private Vector3 movement;

        /// <summary>
        /// Returns vector of last <see cref="GrassPhysicsArea"/> movement
        /// </summary>
        /// <returns>Vector of last GrassPhysicsArea movement</returns>
        public Vector3 GetLastMovementVector()
        {
            return movement;
        }

        /// <summary>
        /// Sets vector of last <see cref="GrassPhysicsArea"/> movement to <see cref="Vector3.zero"/>
        /// </summary>
        public void ResetLastMovementVector()
        {
            movement = Vector3.zero;
        }

        /// <summary>
        /// Moves object to follow target
        /// </summary>
        public void FollowTarget()
        {
            Vector3 newPos = target.position - new Vector3(MathHelper.PosMod(target.position.x, Step_size),
                             0, MathHelper.PosMod(target.position.z, Step_size)) + areaOffset;
            movement += newPos - transform.position;
            transform.position = newPos;
        }

        /// <summary>
        /// Follows target and returns depth texture with post processing
        /// </summary>
        /// <param name="texture"></param>
        public void GetDepthTexture(out Texture texture)
        {
            if ((Time.time - lastUpdateTime) < Time.fixedDeltaTime)
            {
                texture = postProcessTexture;
                return;
            }
            lastUpdateTime = Time.time;

            if (followPlayer && target != null)
            {
                FollowTarget();
            }
            onSetDepthTextureEvent.Invoke();

            postProcessTexture = depthTexture;
            for (int i = 0; i < postProcessProfile.postProcesses.Count; ++i)
            {
                GrassPostProcess postProcess = postProcessProfile.postProcesses[i];
                GrassPostProcessState state = postProcessStates[i];
                postProcess.DoPostProcess(this, ref state, ref postProcessTexture);
                postProcessStates[i] = state;
            }
            ResetLastMovementVector();
            texture = postProcessTexture;
        }

        /// <summary>
        /// Initialization of the needed elements
        /// </summary>
        protected void Awake()
        {
            postProcessStates = new List<GrassPostProcessState>(postProcessProfile.postProcesses.Count);
            for (int i = 0; i < postProcessProfile.postProcesses.Count; ++i)
            {
                GrassPostProcess postProcess = postProcessProfile.postProcesses[i];
                GrassPostProcessState state;
                postProcess.Initialize(this, out state);
                postProcessStates.Add(state);
            }
            transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0));
            if(null != depthTexture)
            {
                textureSize = new Vector2Int(depthTexture.width, depthTexture.height);
            }
        }


#if UNITY_EDITOR
        /// <summary>
        /// Adds function to event
        /// </summary>
        public void AddOnSetDepthTextureEvent(UnityAction func)
        {
            UnityEventTools.AddPersistentListener(onSetDepthTextureEvent, func);
        }
#endif

    }
}