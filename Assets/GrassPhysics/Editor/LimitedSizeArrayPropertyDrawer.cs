using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Specialization of <see cref="LimitedSizeArrayPropertyDrawer{T}"/> for <see cref="GrassActor"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(LimitedSizeArray_GrassActor))]
    public class LimitedSizeArrayPropertyDrawer_GrassActor : LimitedSizeArrayPropertyDrawer<GrassActor>
    {
        protected override string emptyArrayWarning { get { return "You haven't added any grass actor yet!"; } }
        protected override string setElementHint { get { return "Drop here GameObject with GrassActor component that you want to affect the grass."; } }
    }

    /// <summary>
    /// Property drawer of generic class <see cref="LimitedSizeArray{T}"/> 
    /// (to use it make specialization class for both <see cref="LimitedSizeArray{T}"/> and <see cref="LimitedSizeArrayPropertyDrawer{T}"/>)
    /// </summary>
    /// <typeparam name="T">Type of stored class</typeparam>
    public class LimitedSizeArrayPropertyDrawer<T> : PropertyDrawer where T : class
    {
        protected virtual string emptyArrayWarning { get { return "Your array is empty, click button below to add item to the array!"; } }
        protected virtual string setElementHint { get { return "Drop here element you want to store in the array"; } }
        protected float labelHeight = EditorGUIUtility.singleLineHeight;
        protected float helpBoxHeight = 2 * EditorGUIUtility.singleLineHeight;
        protected float elementHeight = EditorGUIUtility.singleLineHeight;
        protected float longButtonHeight = EditorGUIUtility.singleLineHeight;
        protected float smallButtonWidth = 25;
        protected float indent = 15;
        private LimitedSizeArray<T> targetArray;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float sumHeight = 0;
            sumHeight += labelHeight + EditorGUIUtility.standardVerticalSpacing;

            if (!property.isExpanded) return sumHeight;

            if(null == targetArray)
            {
                targetArray = GetTargetArray(property);
            }

            if (targetArray == null)
            {
                Debug.LogError(property.name + " object is null.");
                GUIUtility.ExitGUI();
                return sumHeight;
            }

            if (targetArray.Length <= 0)
            {
                sumHeight += helpBoxHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            for (int i = 0; i < targetArray.Length; ++i)
            {
                sumHeight += elementHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            if (targetArray.Length < targetArray.MaxLength)
            {
                sumHeight += longButtonHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            return sumHeight;
        }


        private string InsertSpacesToString(string input)
        {
            return string.Concat(input.Select(x => Char.IsUpper(x) ? " " + x : x.ToString()).ToArray()).TrimStart(' ');
        }

        private LimitedSizeArray < T > GetTargetArray(SerializedProperty property)
        {
            var parent = property.serializedObject.targetObject;
            System.Type parentType = parent.GetType();
            var field = parentType.GetField(property.propertyPath);
            if (field != null)
            {
                return field.GetValue(parent) as LimitedSizeArray<T>;
            }
            return null;
        }

        private void ShowHelpBoxGUI(ref Rect position)
        {
            Rect helpBoxRect = new Rect(position.x, position.y, position.width, helpBoxHeight);
            EditorGUI.HelpBox(helpBoxRect, emptyArrayWarning, MessageType.Warning);
            position.y += helpBoxRect.height + EditorGUIUtility.standardVerticalSpacing;
        }

        private void ShowElementPropertyGUI(int i, ref Rect position, SerializedProperty property)
        {
            Rect fieldRect = new Rect(position.x, position.y, position.width - smallButtonWidth, elementHeight);

            SerializedProperty fieldProperty = property.FindPropertyRelative("elements").GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(fieldRect,
                                    fieldProperty,
                                    new GUIContent(InsertSpacesToString(typeof(T).Name) + " " + i,
                                    setElementHint));
            Rect smallButtonRect = new Rect(position.x + position.width - smallButtonWidth, position.y, smallButtonWidth, elementHeight);
            if (GUI.Button(smallButtonRect, "-", EditorStyles.miniButton))
            {
                targetArray.RemoveTargetAtIndex(i);
            }
            position.y += fieldRect.height + EditorGUIUtility.standardVerticalSpacing;
        }

        private void ShowAddButtonGUI(ref Rect position)
        {
            Rect longButtonRect = new Rect(position.x, position.y, position.width, longButtonHeight);
            position.y += longButtonRect.height + EditorGUIUtility.standardVerticalSpacing;
            if (targetArray.Length < targetArray.MaxLength && GUI.Button(longButtonRect, "+", EditorStyles.miniButton))
            {
                targetArray.AddTargetToArray(null);
            }
        }

        private void ShowGUI(Rect position, SerializedProperty property)
        {
            position.y += labelHeight + EditorGUIUtility.standardVerticalSpacing;

            if (targetArray == null)
            {
                Debug.LogError(property.displayName + " object is null.");
                GUIUtility.ExitGUI();
                return;
            }
            if (targetArray.Length <= 0)
            {
                ShowHelpBoxGUI(ref position);
            }
            for (int i = 0; i < targetArray.Length; ++i)
            {
                ShowElementPropertyGUI(i, ref position, property);
            }
            ShowAddButtonGUI(ref position);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                                    property.isExpanded, property.displayName, true);

            if(property.isExpanded)
            {
                if (null == targetArray)
                {
                    targetArray = GetTargetArray(property);
                }
                position.x += indent;
                position.width -= indent;
                ShowGUI(position, property);
            }

            EditorGUI.EndProperty();
        }
    }
}
