using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    [CustomPropertyDrawer(typeof(GrassPostProcess), true)]
    public class GrassPostProcessDrawer : GenericPropertyDrawer
    {
        private GrassPostProcess GetElement(SerializedProperty property)
        {
            GrassPostProcess element = property.objectReferenceValue as GrassPostProcess;

            if (element == null)
            {
                (property.serializedObject.targetObject as GrassPostProcessProfile).postProcesses.RemoveAll(item => item == null);
            }

            return element;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            GrassPostProcess element = GetElement(property);

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
                                                    property.isExpanded,
                                                    new GUIContent(NameAttribute.GetNameFromClassType(element.GetType()), NameAttribute.GetDescriptionFromClassType(element.GetType())),
                                                    true);


            if (GUI.changed) property.serializedObject.ApplyModifiedProperties();
            if (property.objectReferenceValue == null) EditorGUIUtility.ExitGUI();

            if (property.isExpanded)
            {
                const float witdthOffset = 25f;
                var newRect = new Rect(position.x, position.y, position.width - position.x - witdthOffset, position.height);
                DrawAllChildrensGUI(newRect, property);
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}
