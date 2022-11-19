using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace ShadedTechnology.GrassPhysics
{
    [System.Serializable]
    public class OptionalFloat
    {
        public bool enabled;
        [SerializeField]
        private float value;
        public float Value
        {
            get
            {
                return enabled ? value : 0;
            }
            set
            {
                this.value = value;
            }
        }

        public OptionalFloat(float value)
        {
            this.value = value;
        }

        public OptionalFloat(bool enabled, float value)
        {
            this.enabled = enabled;
            this.value = value;
        }

#if UNITY_EDITOR
        public System.Action onValueChange;
        public void InvokeOnValueChange()
        {
            onValueChange.Invoke();
        }
#endif
    }
}
