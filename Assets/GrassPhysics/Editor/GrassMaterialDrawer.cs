using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace ShadedTechnology.GrassPhysics
{
    [CustomPropertyDrawer(typeof(GrassMaterialProfile), true)]
    public class GrassMaterialDrawer : GenericPropertyDrawer
    {
        private void CreateNewProfile(Type type, SerializedProperty property)
        {
            GrassMaterialProfile profile = AssetsManager.CreateNewScriptableObjectOfType(type,
                                                                                  "Grass Material location",
                                                                                  AssetsManager.GetScenePath(property),
                                                                                  GrassMaterialAttribute.GetFileNameFromClassType(type),
                                                                                  "asset") as GrassMaterialProfile;
            if (null != profile)
            {
                property.objectReferenceValue = profile;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void CreateNewMaterialProfileMenu(SerializedProperty property)
        {
            List<Type> types = AssetsManager.GetListOfType(typeof(GrassMaterialProfile));
            GenericMenu menu = new GenericMenu();
            foreach (Type type in types)
            {
                menu.AddItem(new GUIContent(GrassMaterialAttribute.GetNameFromClassType(type)), false, () => { CreateNewProfile(type, property); });
            }
            menu.ShowAsContext();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            position.height = EditorGUI.GetPropertyHeight(property, label, true);
            EditorGUIHelper.ProfilePropertyAndNewButtonGUI(position, property, label, () => CreateNewMaterialProfileMenu(property));

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            property.isExpanded = true;
            if (property.objectReferenceValue == null) return;
            DrawAllChildrensGUI(position, property);
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
