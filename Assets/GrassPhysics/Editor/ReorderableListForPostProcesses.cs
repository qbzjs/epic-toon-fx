using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics {

    public class ReorderableListForPostProcesses {

        private ReorderableList m_ReorderableList;
        private GrassPostProcessProfile m_Profile;

        public ReorderableListForPostProcesses(ReorderableList reorderable, GrassPostProcessProfile profile)
        {
            m_Profile = profile;
            m_ReorderableList = reorderable;
            m_ReorderableList.displayAdd = true;
            m_ReorderableList.drawHeaderCallback = DrawHeaderCallback;
            m_ReorderableList.drawElementCallback = DrawElementCallback;
            m_ReorderableList.onAddDropdownCallback = OnAddDropdownCallback;
            m_ReorderableList.onRemoveCallback = OnRemoveCallback;
            m_ReorderableList.elementHeightCallback = ElementHeightCallback;
            m_ReorderableList.onReorderCallbackWithDetails = OnReorderableWithDetails;
        }

        private void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Post Processes");
        }

        private void OnAddDropdownCallback(Rect button, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();
#if UNITY_2018_2_OR_NEWER
            menu.allowDuplicateNames = true;
#endif
            List<Type> types = AssetsManager.GetListOfType(typeof(GrassPostProcess));
            foreach (Type type in types)
            {
                menu.AddItem(new GUIContent(NameAttribute.GetNameFromClassType(type), NameAttribute.GetDescriptionFromClassType(type)), false, AddPostProcessHandler, type.Name);
            }
            menu.ShowAsContext();
        }

        private void AddPostProcessHandler(object target)
        {
            GrassPostProcess obj = ScriptableObject.CreateInstance(target as string) as GrassPostProcess;
            m_Profile.postProcesses.Add(obj as GrassPostProcess);
            AssetDatabase.AddObjectToAsset(obj, m_Profile);
            AssetDatabase.SaveAssets();
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {    
            SerializedProperty element = m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(position: new Rect(rect.x += 10, rect.y, Screen.width * .8f, height: EditorGUIUtility.singleLineHeight),
                                    property: element, label: GUIContent.none, includeChildren: true);
        }

        private void OnReorderableWithDetails(ReorderableList list, int oldIndex, int newIndex)
        {
            SerializedProperty prop1 = list.serializedProperty.GetArrayElementAtIndex(oldIndex);
            SerializedProperty prop2 = list.serializedProperty.GetArrayElementAtIndex(newIndex);

            bool tmp = prop1.isExpanded;
            prop1.isExpanded = prop2.isExpanded;
            prop2.isExpanded = tmp;
        }

        private float ElementHeightCallback(int index)
        {
            float propertyHeight = EditorGUI.GetPropertyHeight(m_ReorderableList.serializedProperty.GetArrayElementAtIndex(index), true);
            float spacing = EditorGUIUtility.singleLineHeight / 2;
            return propertyHeight + spacing;
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            UnityEngine.Object.DestroyImmediate(m_Profile.postProcesses[list.index], true);
            m_Profile.postProcesses.RemoveAt(list.index);
            AssetDatabase.SaveAssets();
        }
    }

}
