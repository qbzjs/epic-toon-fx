using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Attribute to store GrassMaterial surface type and path to include file with fragment shader function
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class GrassMaterialAttribute : Attribute
    {
        public string surf { get; set; }
        public string fragmentShader { get; set; }
        public string name { get; set; }
        public string fileName { get; set; }

        public static GrassMaterialAttribute GetAttributeFromClassType(Type type)
        {
            if (type == null) return null;
            return (type.GetCustomAttributes(typeof(GrassMaterialAttribute), false).FirstOrDefault() as GrassMaterialAttribute);
        }

        public static string GetNameFromClassType(Type type)
        {
            GrassMaterialAttribute attr = GetAttributeFromClassType(type);
            if (attr == null || string.IsNullOrEmpty(attr.name))
            {
                return type.Name;
            }
            return attr.name;
        }

        public static string GetFileNameFromClassType(Type type)
        {
            GrassMaterialAttribute attr = GetAttributeFromClassType(type);
            if (attr == null || string.IsNullOrEmpty(attr.fileName))
            {
                return type.Name;
            }
            return attr.fileName;
        }

        public static string GetSurfFromClassType(Type type)
        {
            GrassMaterialAttribute attr = GetAttributeFromClassType(type);
            if (attr == null || string.IsNullOrEmpty(attr.surf))
            {
                return "Lambert";
            }
            return attr.surf;
        }

        public static string GetFragmentShaderFromClassType(Type type)
        {
            GrassMaterialAttribute attr = GetAttributeFromClassType(type);
            if (attr == null || string.IsNullOrEmpty(attr.fragmentShader))
            {
                return "../LegacyRP/GrassSurfaces/GrassLambertianSurface.cginc";
            }
            return attr.fragmentShader;
        }
    }
}
