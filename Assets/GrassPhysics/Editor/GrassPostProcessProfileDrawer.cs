using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadedTechnology.GrassPhysics
{
    [CustomPropertyDrawer(typeof(GrassPostProcessProfile))]
    public class GrassPostProcessProfileDrawer : PropertyDrawer
    {
        private ReorderableList reorderableList;
        ReorderableListForPostProcesses postProcessesListHandler;

        /// <summary>
        /// Returns <see cref="ReorderableList"/> from <see cref="GrassPostProcess"/> list and creates 
        /// <see cref="GrassPostProcessProfile.postProcesses"/> list and <see cref="reorderableList"/> objects
        /// if either of them is null
        /// </summary>
        /// <param name="myTarget">Target <see cref="GrassPostProcessProfile"/> object</param>
        /// <param name="profileSerialized"><see cref="SerializedObject"/> of target profile</param>
        /// <returns><see cref="ReorderableList"/> for <see cref="GrassPostProcessProfile.postProcesses"/></returns>
        private ReorderableList GetPostProcesses(GrassPostProcessProfile myTarget, SerializedObject profileSerialized)
        {
            if (myTarget.postProcesses == null)
            {
                myTarget.postProcesses = new List<GrassPostProcess>();
            }

            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(profileSerialized, profileSerialized.FindProperty("postProcesses"), true, true, true, true);
            }
            reorderableList.serializedProperty = profileSerialized.FindProperty("postProcesses");
            return reorderableList;
        }

        /// <summary>
        /// Shows <see cref="ReorderableList"/> of <see cref="GrassPostProcessProfile.postProcesses"/>
        /// </summary>
        /// <param name="myTarget">Target <see cref="GrassPostProcessProfile"/> object</param>
        /// <param name="property"><see cref="GrassPostProcessProfile"/> property</param>
        private void PostProcessesGUI(GrassPostProcessProfile myTarget, SerializedProperty property)
        {
            SerializedObject profileSerialized = new SerializedObject(myTarget);
            profileSerialized.Update();
            if (postProcessesListHandler == null)
            {
                postProcessesListHandler = new ReorderableListForPostProcesses(GetPostProcesses(myTarget, profileSerialized), myTarget);
            }
            GetPostProcesses(myTarget, profileSerialized).DoLayoutList();
            profileSerialized.ApplyModifiedProperties();
        }

        private void CreateNewProfile(SerializedProperty property)
        {
            GrassPostProcessProfile profile = AssetsManager.CreateNewScriptableObjectOfType<GrassPostProcessProfile>("Grass Post Process Profile location",
                                                                                    AssetsManager.GetScenePath(property),
                                                                                    "GrassPostProcessProfile",
                                                                                    "asset");
            if (null != profile)
            {
                property.objectReferenceValue = profile;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIHelper.ProfilePropertyAndNewButtonGUI(position, property, label, () => CreateNewProfile(property));

            GrassPostProcessProfile myTarget = (property.objectReferenceValue as GrassPostProcessProfile);
            if (null == myTarget) return;

            PostProcessesGUI(myTarget, property);
        }
    }
}
