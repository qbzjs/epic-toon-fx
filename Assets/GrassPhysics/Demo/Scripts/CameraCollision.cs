using UnityEngine;

namespace ThirdPersonCamera
{

    public class CameraCollision : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float minDistance = 1.6f;
        [SerializeField] private float maxDistance = 2.9f;
        [SerializeField] private float smoothBase = 8f;
        [SerializeField] private float sphereRadius = 0.5f;

        private Vector3 dollyDirBase;
        private float distance;

        private void Awake()
        {
            Init();
            SetLocalPosition();
        }

        private void Update()
        {
            SetLocalPosition();
        }

        private void Init()
        {
            dollyDirBase = transform.localPosition.normalized;
            distance = transform.localPosition.magnitude;
        }

        private void SetLocalPosition()
        {
            RaycastHit hit;
            if (HasLinecastCollision(out hit))
            {
                SetDistance(hit.distance);
                SetLocalPositionByLinecastHit(hit);
            }
            else
            {
                HandleDistanceBySphereCastCollison();
                SetLocalPositionSmoothlyByDistance();
            }
        }

        private void SetDistance(float distance)
        {
            this.distance = distance;
        }

        private void SetDistanceToMax()
        {
            SetDistance(maxDistance);
        }

        private bool IsVeryClose()
        {
            return distance < minDistance;
        }

        private bool HasLinecastCollision(out RaycastHit hit)
        {
            var start = transform.parent.position;
            var end = transform.parent.TransformPoint(dollyDirBase * maxDistance);

            return Physics.Linecast(start, end, out hit) && hit.collider.tag != "Player";
        }

        private void SetLocalPositionByLinecastHit(RaycastHit hit)
        {
            transform.localPosition = GetLocallPositionByLinecastHit(hit);
        }

        private Vector3 GetLocallPositionByLinecastHit(RaycastHit hit)
        {
            var localPos = transform.parent.transform.InverseTransformPoint(hit.point);

            if (IsVeryClose()) { localPos.y += minDistance - distance; }
            return localPos;
        }

        private void HandleDistanceBySphereCastCollison()
        {
            RaycastHit hit;
            if (HasSphereCastCollision(out hit))
            {
                SetDistanceBySphereCastHit(hit);
            }
            else
            {
                SetDistanceToMax();
            }
        }

        private bool HasSphereCastCollision(out RaycastHit hit)
        {
            var origin = transform.parent.transform.position;
            var direction = transform.parent.transform.TransformDirection(transform.localPosition);

            var ray = new Ray(origin, direction);
            return Physics.SphereCast(ray, sphereRadius, out hit, maxDistance) && hit.collider.tag != "Player";
        }

        private void SetDistanceBySphereCastHit(RaycastHit hit)
        {
            var sphereDiameter = sphereRadius * 2f;
            SetDistance(hit.distance + sphereDiameter);
        }

        private void SetLocalPositionSmoothlyByDistance()
        {
            var start = transform.localPosition;
            var end = GetDollyDir() * distance;
            var interpolant = GetSmooth() * Time.deltaTime;

            transform.localPosition = Vector3.Lerp(start, end, interpolant);
        }

        private Vector3 GetDollyDir()
        {
            var dollyDir = dollyDirBase;

            if (IsVeryClose())
            {
                var quarterSphereRadius = sphereRadius / 4;
                dollyDir.y += minDistance - distance + quarterSphereRadius;
            }
            return dollyDir;
        }

        private float GetSmooth()
        {
            var speed = Input.GetAxisRaw("Mouse X");

            if (speed >= 1) { return smoothBase * speed; }
            return smoothBase;
        }

    }

}