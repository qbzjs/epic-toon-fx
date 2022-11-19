using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    public class EditorGUIHelper
    {
        public static GUIStyle foldoutStyle
        {
            get
            {
                GUIStyle style = EditorStyles.foldout;
                style.fontStyle = FontStyle.Bold;
                return style;
            }
        }

        /// <summary>
        /// Shows default property and the "New" button on the right side
        /// </summary>
        /// <param name="position"><see cref="Rect"/> position of property with button</param>
        /// <param name="property"><see cref="ScriptableObject"/> property</param>
        /// <param name="label"><see cref="ScriptableObject"/> property label</param>
        /// <param name="onNewButton"><see cref="Action"/> called on new button press</param>
        public static void ProfilePropertyAndNewButtonGUI(Rect position,
                                                          SerializedProperty property,
                                                          GUIContent label,
                                                          Action onNewButton)
        {
            const int buttonWidth = 45;
            const int horizontalSpace = 5;
            Rect fieldRect = new Rect(position.x, position.y, position.width - buttonWidth - horizontalSpace, position.height);
            Rect buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);
            EditorGUI.PropertyField(fieldRect, property, label);

            if (GUI.Button(buttonRect, new GUIContent("New", "Create a new profile."), EditorStyles.miniButton))
            {
                onNewButton.Invoke();
            }
        }

        public static object GetPropertyParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetObjectValue(obj, elementName, index);
                }
                else
                {
                    obj = GetObjectValue(obj, element);
                }
            }
            return obj;
        }

        public static object GetObjectValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        public static object GetObjectValue(object source, string name, int index)
        {
            var enumerable = GetObjectValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
    }
}