using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    public class GenericPropertyDrawer : PropertyDrawer
    {
        readonly Vector2 textureFieldSize = new Vector2(78, 60);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            var data = property.objectReferenceValue;
            if (property.isExpanded && null != data)
            {
                SerializedObject serializedObject = new SerializedObject(data);
                SerializedProperty prop = serializedObject.GetIterator();
                if (prop.NextVisible(true))
                {
                    do
                    {
                        if (prop.name == "m_Script") continue;
                        if (typeof(Texture).IsAssignableFrom(GetPropertyType(prop)))
                        {
                            totalHeight += textureFieldSize.y + EditorGUIUtility.standardVerticalSpacing;
                        }
                        else
                        {
                            float height = EditorGUI.GetPropertyHeight(prop, null, true) + EditorGUIUtility.standardVerticalSpacing;
                            totalHeight += height;
                        }
                    }
                    while (prop.NextVisible(false));
                }
                // Add a tiny bit of height if open for the background
                totalHeight += EditorGUIUtility.standardVerticalSpacing;
            }
            return totalHeight;
        }

        public void DrawAllChildrensGUI(Rect position, SerializedProperty property)
        {
            EditorGUI.indentLevel++;
            SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);

            // Iterate over all the values and draw them
            SerializedProperty prop = serializedObject.GetIterator();
            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (prop.NextVisible(true))
            {
                do
                {
                    // Don't bother drawing the class file
                    if (prop.name == "m_Script") continue;
                    if (typeof(Texture).IsAssignableFrom(GetPropertyType(prop)))
                    {
                        DrawTextureGUI(ref y, position, prop);
                    }
                    else
                    {
                        float height = EditorGUI.GetPropertyHeight(prop, new GUIContent(prop.displayName), true);
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), prop, true);
                        y += height + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                while (prop.NextVisible(false));
            }
            if (GUI.changed)
                serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel--;
        }

        public Type GetPropertyType(SerializedProperty property)
        {
            string[] parts = property.propertyPath.Split('.');

            Type currentType = property.serializedObject.targetObject.GetType();

            for (int i = 0; i < parts.Length; i++)
            {
                currentType = currentType.GetField(parts[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).FieldType;
            }

            return currentType;
        }

        private void DrawTextureGUI(ref float prevHeight, Rect position, SerializedProperty property)
        {

            GUIContent label = new GUIContent(property.displayName);
            Rect labelRect = new Rect(position.x,
                                      prevHeight,
                                      position.width,
                                      EditorGUI.GetPropertyHeight(property, label, true));
            EditorGUI.LabelField(labelRect, label);
            Rect newRect = new Rect(position.xMax - textureFieldSize.x,
                                    prevHeight,
                                    textureFieldSize.x,
                                    textureFieldSize.y);
            prevHeight += newRect.height + EditorGUIUtility.standardVerticalSpacing;
            property.objectReferenceValue = EditorGUI.ObjectField(newRect, property.objectReferenceValue, typeof(Texture2D), false);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
                                                    property.isExpanded,
                                                    label,
                                                    true);


            if (GUI.changed) property.serializedObject.ApplyModifiedProperties();
            if (property.objectReferenceValue == null) return;

            if (property.isExpanded)
            {
                DrawAllChildrensGUI(position, property);
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}