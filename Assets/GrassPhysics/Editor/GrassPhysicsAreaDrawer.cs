using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    [CustomPropertyDrawer(typeof(GrassPhysicsArea))]
    public class GrassAreaDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing;
            if (property.objectReferenceValue == null)
            {
                totalHeight += 3 * (EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing);
            }
            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            float baseHeight = EditorGUI.GetPropertyHeight(property, label, true);
            position.height = baseHeight;
            EditorGUI.PropertyField(position, property, label);
            
            if (property.objectReferenceValue == null)
            {
                Rect helpBoxRect = new Rect(position.x, position.y + baseHeight + EditorGUIUtility.standardVerticalSpacing, position.width, baseHeight * 2);
                EditorGUI.HelpBox(helpBoxRect, "Grass Physics Area is not assigned!", MessageType.Warning);
                Rect buttonRect = new Rect(position.x, helpBoxRect.y + 2 * baseHeight + EditorGUIUtility.standardVerticalSpacing, position.width, baseHeight);
                if (GUI.Button(buttonRect, "Create grass physics area"))
                {
                    GameObject instance = new GameObject("GrassPhysicsArea", typeof(GrassPhysicsArea));
                    property.objectReferenceValue = instance.GetComponent<GrassPhysicsArea>();
                    Selection.activeGameObject = instance;
                }
            }
            EditorGUI.EndProperty();
        }
    }
}
