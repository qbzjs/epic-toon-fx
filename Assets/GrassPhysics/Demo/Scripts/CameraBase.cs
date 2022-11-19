using UnityEngine;

namespace ThirdPersonCamera
{

    public class CameraBase : MonoBehaviour
    {
        [Tooltip("GameObject that will serve as a point of view for the camera." +
            " It should be an object within the character the camera is looking at.")]
        [SerializeField] private Transform target;

        [Header("Parameters")]
        [SerializeField] private float movementSpeed = 120f;
        [SerializeField] private float maxClampAngle = 80f;
        [SerializeField] private float minClampAngle = -80f;
        [SerializeField] private float inputSensitivity = 150f;
        [SerializeField] private bool disableCursor = true;
        [SerializeField] private bool mobileInput = false;

        private const float RotZ = 0f;

        private float rotY;
        private float rotX;

        private void Awake()
        {
            Init();
            if (disableCursor) SetupCursor();
            SetRotation();
            SetPosition();
        }

        private void Update()
        {
            if (!mobileInput) SetRotation();
        }

        private void LateUpdate()
        {
            SetPosition();
        }

        private void Init()
        {
            var rot = transform.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
        }

        private void SetupCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void SetRotation()
        {
            UpdateRotY();
            UpdateRotX();

            Quaternion rot = Quaternion.Euler(rotX, rotY, RotZ);
            transform.rotation = rot;
        }

        public void SetRotation(Vector2 rotation)
        {
            rotY += rotation.x * inputSensitivity * Time.deltaTime;
            rotX += -rotation.y * inputSensitivity * Time.deltaTime;
            rotX = Mathf.Clamp(rotX, minClampAngle, maxClampAngle);

            Quaternion rot = Quaternion.Euler(rotX, rotY, RotZ);
            transform.rotation = rot;
        }

        private void UpdateRotY()
        {
            var mouseX = Input.GetAxisRaw("Mouse X");

            var finalInputX = mouseX;

            rotY += finalInputX * inputSensitivity * Time.deltaTime;
        }

        private void UpdateRotX()
        {
            var mouseY = -Input.GetAxisRaw("Mouse Y");

            var finalInputZ = mouseY;

            rotX += finalInputZ * inputSensitivity * Time.deltaTime;
            rotX = Mathf.Clamp(rotX, minClampAngle, maxClampAngle);
        }

        private void SetPosition()
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                GetMaxDistanceDelta()
            );
        }

        private float GetMaxDistanceDelta()
        {
            return movementSpeed * Time.deltaTime;
        }

    }

}