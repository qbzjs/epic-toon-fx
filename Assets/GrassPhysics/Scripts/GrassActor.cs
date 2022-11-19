using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class used to define objects that affects the grass in Simple Physics Mode
    /// </summary>
    [AddComponentMenu("Grass Physics/GrassActor")]
    public class GrassActor : MonoBehaviour
    {
        public Vector3 offset;
        public float radius = 2;

        public Vector4 GetVector4()
        {
            return new Vector4(transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z + offset.z, radius);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position + offset, radius);
        }
#endif
    }
}