using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Contains useful editor functions
    /// </summary>
    public static class EditorHelper
    {
        /// <summary>
        /// Creates <see cref="LayerMask"/> field using <see cref="EditorGUILayout"/>
        /// </summary>
        /// <param name="label">Label of the field</param>
        /// <param name="layerMask">Target <see cref="LayerMask"/></param>
        /// <returns>Resultant <see cref="LayerMask"/></returns>
        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            bool isEverything = true;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) != 0)
                {
                    maskWithoutEmpty |= (1 << i);
                }
                else
                {
                    isEverything = false;
                }
            }
            if (isEverything) maskWithoutEmpty = ~0;
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            isEverything = true;
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) != 0)
                {
                    mask |= (1 << layerNumbers[i]);
                }
                else
                {
                    isEverything = false;
                }
            }
            if (isEverything) mask = ~0;
            layerMask.value = mask;
            return layerMask;
        }
    }
}
