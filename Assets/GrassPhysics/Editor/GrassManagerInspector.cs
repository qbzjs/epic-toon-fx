using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

namespace ShadedTechnology.GrassPhysics
{
    [CustomEditor(typeof(GrassManager))]
    public class GrassManagerInspector : Editor
    {
        private static readonly string[] tabNames = { "Only Wind", "Simple Physics", "Full Physics" };
        
        private const string fullPhysicsWarningForLowEndPlatform =
            "On some platforms Full Physics Mode may not working.";
        private const string multipleGrassManagersWarning = "There are more than one Grass Managers in the scene."
                                        + " Please ensure there is always exactly one Grass Manager in the scene.";

        GrassManager myTarget;
        
        private bool materialFoldout = true;
        private bool physicsFoldout = true;
        
        /// <summary>
        /// Returns <see cref="GrassPhysicsMode"/> as int
        /// based on what type of physics script is currently attached to manager object,
        /// <para> not based on grassMode stored in manager object!</para>
        /// </summary>
        /// <returns><see cref="GrassPhysicsMode"/> in int</returns>
        private int GetCurrentTab()
        {
            if(myTarget.grassPhysics != null)
            {
                if (myTarget.grassPhysics is GrassFullPhysics)
                {
                    return (int)GrassPhysicsMode.Full;
                }
                else if (myTarget.grassPhysics is GrassSimplePhysics)
                {
                    return (int)GrassPhysicsMode.Simple;
                }
            }
            return (int)GrassPhysicsMode.None;
        }

        /// <summary>
        /// Sets grass mode to what type of script is currently attached to grass manager object
        /// </summary>
        private void CorrectTabs()
        {
            myTarget.grassMode = (GrassPhysicsMode)GetCurrentTab();
        }

        /// <summary>
        /// Returns true if physics mode and physics script type are the same
        /// </summary>
        /// <returns>True if physics mode and physics script type are the same</returns>
        private bool IsCorrectScriptType()
        {
            return (GetCurrentTab() == (int)myTarget.grassMode);
        }

        private void ChangePhysicsScriptTo<T>() where T : GrassPhysics
        {
            if (myTarget.grassPhysics == null || myTarget.grassPhysics.GetType() != typeof(T))
            {
                if (myTarget.grassPhysics != null)
                {
                    DestroyImmediate(myTarget.grassPhysics, true);
                }
                myTarget.grassPhysics = myTarget.gameObject.AddComponent<T>();
            }
        }

        /// <summary>
        /// Changes grass physics script to correspond to grass physics mode if it doesn't
        /// </summary>
        private void SetCorrectScript()
        {
            switch (myTarget.grassMode)
            {
                case GrassPhysicsMode.Simple:
                    {
                        ChangePhysicsScriptTo<GrassSimplePhysics>();
                        break;
                    }
                case GrassPhysicsMode.Full:
                    {
                        ChangePhysicsScriptTo<GrassFullPhysics>();
                        break;
                    }
                default:
                    {
                        if (myTarget.grassPhysics != null)
                        {
                            DestroyImmediate(myTarget.grassPhysics, true);
                        }
                        break;
                    }
            }

            myTarget.CorrectShaderSettings();
            GUIUtility.ExitGUI();
        }

        private void ShowGUI()
        {
            UrpApplyWindow.HandleURP();

            materialFoldout = EditorGUILayout.Foldout(materialFoldout, "Material", true, EditorGUIHelper.foldoutStyle);
            if (materialFoldout)
            {
                EditorGUILayout.PropertyField(new SerializedObject(myTarget).FindProperty("grassMaterial"));

                if(myTarget.grassMaterial != null)
                {
                    myTarget.grassMaterial.SetMaterialToGrass();
                }
                myTarget.CorrectGrassMaterialSettings();
            }
            EditorGUILayout.Space();

            physicsFoldout = EditorGUILayout.Foldout(physicsFoldout, "Physics", true, EditorGUIHelper.foldoutStyle);
            if (physicsFoldout)
            {
                myTarget.grassPhysics = EditorGUILayout.ObjectField("Grass Physics Script", myTarget.grassPhysics, typeof(GrassPhysics), true) as GrassPhysics;

                CorrectTabs();
                myTarget.grassMode = (GrassPhysicsMode)GUILayout.Toolbar((int)myTarget.grassMode, tabNames);

                if (myTarget.grassMode == GrassPhysicsMode.Full && AssetsManager.IsLowEndPlatform())
                {
                    EditorGUILayout.HelpBox(fullPhysicsWarningForLowEndPlatform, MessageType.Warning);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            myTarget = target as GrassManager;
            Undo.RecordObject(myTarget, "Update in " + myTarget.name);

            if (FindObjectsOfType<GrassManager>().Length > 1)
            {
                EditorGUILayout.HelpBox(multipleGrassManagersWarning, MessageType.Error);
                Debug.LogError(multipleGrassManagersWarning);
            }

            EditorGUI.BeginChangeCheck();
            ShowGUI();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(myTarget);
            }
            serializedObject.ApplyModifiedProperties();


            if (IsCorrectScriptType() == false)
            {
                SetCorrectScript();
            }
        }
    }
}
