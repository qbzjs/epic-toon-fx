using System;
using System.Linq;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Attribute to name classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class NameAttribute : Attribute
    {
        public string name { get; set; }
        public string description { get; set; }

        public NameAttribute(string name)
        {
            this.name = name;
            this.description = "";
        }

        public NameAttribute(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public static NameAttribute GetAttributeFromClassType(Type type)
        {
            return (type.GetCustomAttributes(typeof(NameAttribute), false).FirstOrDefault() as NameAttribute);
        }

        public static string GetNameFromClassType(Type type)
        {
            NameAttribute attr = GetAttributeFromClassType(type);
            if (attr == null || string.IsNullOrEmpty(attr.name))
            {
                return type.Name;
            }
            return attr.name;
        }

        public static string GetDescriptionFromClassType(Type type)
        {
            NameAttribute attr = GetAttributeFromClassType(type);
            if (attr == null || string.IsNullOrEmpty(attr.description))
            {
                return "";
            }
            return attr.description;
        }
    }
}