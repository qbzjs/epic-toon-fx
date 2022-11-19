using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    [CreateAssetMenu(fileName = "GrassPostProcessProfile", menuName = "GrassPhysics/GrassPostProcessProfile")]
    public class GrassPostProcessProfile : ScriptableObject
    {
        [SerializeField]
        public List<GrassPostProcess> postProcesses = new List<GrassPostProcess>();
    }
}
