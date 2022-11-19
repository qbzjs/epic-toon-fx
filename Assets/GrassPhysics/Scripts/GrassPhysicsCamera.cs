using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Sets target texture to camera and grass physics area
    /// </summary>
    [AddComponentMenu("Grass Physics/GrassPhysicsCamera")]
    [RequireComponent(typeof(Camera))]
    public class GrassPhysicsCamera : MonoBehaviour
    {
        public Camera Cam
        {
            get
            {
                if(_cam == null)
                {
                    _cam = GetComponent<Camera>();
                }
                return _cam;
            }
            set
            {
                _cam = value;
            }
        }
        private Camera _cam;

        public Vector2Int textureSize = new Vector2Int(1024, 1024);
        public GrassPhysicsArea physicsArea;

        /// <summary>
        /// Renders to target texture
        /// </summary>
        public void RenderTexture()
        {
            Cam.Render();
        }

        /// <summary>
        /// Destroys camera target texture
        /// </summary>
        public void DestroyCameraTargetTexture()
        {
            RenderTexture temp = Cam.targetTexture;
            Cam.targetTexture = null;
            physicsArea.depthTexture = null;
            DestroyImmediate(temp);
        }

        /// <summary>
        /// Sets target texture for camera and grass physics area
        /// </summary>
        public void SetCameraTargetTexture()
        {
            if(Cam.targetTexture != null)
            {
                DestroyCameraTargetTexture();
            }
            Cam.targetTexture = new RenderTexture(textureSize.x, textureSize.y, 24, RenderTextureFormat.Depth);
            physicsArea.depthTexture = Cam.targetTexture;
        }

        /// <summary>
        /// Returns true if camera's target texture has the same resolution as its set on physics area component
        /// </summary>
        /// <returns>True if camera's target texture has the same resolution as its set on physics area component</returns>
        public bool IsGoodTextureSize()
        {
            return (Cam.targetTexture != null &&
                Cam.targetTexture.width == textureSize.x &&
                Cam.targetTexture.height == textureSize.y);
        }

        /// <summary>
        /// Sets camera settings to fit physics area size
        /// </summary>
        /// <param name="areaSize">Physics area size</param>
        public void SetCameraSettings()
        {
            Vector3 areaSize = physicsArea.areaSize;
            Cam.enabled = false;
            Cam.orthographic = true;
            Cam.allowHDR = false;
            Cam.allowMSAA = false;
            Cam.backgroundColor = Color.black;
            Cam.clearFlags = CameraClearFlags.Depth;
            Cam.renderingPath = RenderingPath.Forward;
            Cam.orthographicSize = areaSize.z / 2f;
            Cam.farClipPlane = areaSize.y;
            
            if (Event.current == null || Event.current.type == EventType.Repaint)
            {
                Cam.depthTextureMode = DepthTextureMode.Depth;
            }

        }

        /// <summary>
        /// Initialization of the needed elements
        /// </summary>
        protected void Awake()
        {
            SetCameraSettings();
            if (Cam.targetTexture == null || !IsGoodTextureSize())
            {
                SetCameraTargetTexture();
            }
            else
            {
                physicsArea.depthTexture = Cam.targetTexture;
            }

        }
    }
}
