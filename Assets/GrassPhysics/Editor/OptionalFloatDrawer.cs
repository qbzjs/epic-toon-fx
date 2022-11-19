using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    [CustomPropertyDrawer(typeof(OptionalFloat))]
    public class OptionalFloatDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float standardHeight = EditorGUI.GetPropertyHeight(property, true);
            float totalHeight = standardHeight;
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            float standardHeight = EditorGUI.GetPropertyHeight(property, true);
            position.height = standardHeight;
            SerializedProperty propEnabled = property.FindPropertyRelative("enabled");
            SerializedProperty propValue = property.FindPropertyRelative("value");

            GUIContent toggleLabel = propEnabled.boolValue ? new GUIContent() : label;
            Rect toggleRect = position;
            if (propEnabled.boolValue) toggleRect.width = 15;
            propEnabled.boolValue = EditorGUI.ToggleLeft(toggleRect, toggleLabel, propEnabled.boolValue);
            position.x += 17;
            if (propEnabled.boolValue)
            {
                propValue.floatValue = EditorGUI.FloatField(position, label, propValue.floatValue);
            }
            OptionalFloat target = EditorGUIHelper.GetPropertyParent(propValue) as OptionalFloat;
            if (target.onValueChange != null)
            {
                target.onValueChange.Invoke();
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}
