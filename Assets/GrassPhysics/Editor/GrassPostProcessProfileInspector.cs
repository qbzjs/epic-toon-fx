using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    [CustomEditor(typeof(GrassPostProcessProfile))]
    public class GrassPostProcessProfileInspector : Editor
    {
        private ReorderableList reorderableList;
        private ReorderableList getPostProcesses()
        {
            if (myTarget.postProcesses == null)
            {
                myTarget.postProcesses = new List<GrassPostProcess>();
            }

            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("postProcesses"), true, true, true, true);
            }
            return reorderableList;
        }
        ReorderableListForPostProcesses postProcessesListHandler;

        private void ShowGUI()
        {
            serializedObject.Update();
            if (postProcessesListHandler == null)
            {
                postProcessesListHandler = new ReorderableListForPostProcesses(getPostProcesses(), myTarget);
            }
            getPostProcesses().DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }


        GrassPostProcessProfile myTarget;

        public override void OnInspectorGUI()
        {
            myTarget = target as GrassPostProcessProfile;
            Undo.RecordObject(myTarget, "Update in " + myTarget.name);

            EditorGUI.BeginChangeCheck();
            ShowGUI();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(myTarget);
            }
        }
    }
}
