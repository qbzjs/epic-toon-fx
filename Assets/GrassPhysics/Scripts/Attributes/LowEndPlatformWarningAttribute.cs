using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Use this attribute with message to show if the target build platform is low end (diffrent than Windows, XboxOne, PS4)
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class LowEndPlatformWarningAttribute : PropertyAttribute
    {
        public string warning;

        public LowEndPlatformWarningAttribute(string warning)
        {
            this.warning = warning;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LowEndPlatformWarningAttribute))]
    public class LowEndPlatformWarningDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float standardHeight = EditorGUI.GetPropertyHeight(property, true);
            float totalHeight = standardHeight;
            if (AssetsManager.IsLowEndPlatform())
            {
                totalHeight += 2 * standardHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float standardHeight = EditorGUI.GetPropertyHeight(property, true);
            position.height = standardHeight;
            if (AssetsManager.IsLowEndPlatform())
            {
                LowEndPlatformWarningAttribute warningAttribute = attribute as LowEndPlatformWarningAttribute;
                Rect warningRect = new Rect(position.x, position.y, position.width, standardHeight * 2);
                EditorGUI.HelpBox(warningRect, warningAttribute.warning, MessageType.Warning);
                position.y += warningRect.height + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.PropertyField(position, property, label);
        }
    }
#endif
}