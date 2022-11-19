using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Scriptable object storing grass mode settings
    /// </summary>
    public class GrassSettings : ScriptableObject
    {
        public bool useAuto;
        public bool useSimple;
        public bool useFull;
    }
}
