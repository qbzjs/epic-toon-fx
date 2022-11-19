using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Creates window with grass settings
    /// </summary>
    public class GrassWindow : EditorWindow
    {
        private void ResetShaderSettings(GrassSettings grassSettings, GrassManager grassManager)
        {
            if (grassSettings == null) return;
            if (grassSettings.useAuto)
            {
                if (grassManager == null) return;
                EditorUtility.SetDirty(grassSettings);
                AssetDatabase.SaveAssets();
                grassManager.CorrectShaderSettings();
                grassManager.CorrectGrassMaterialSettings();
                if (grassManager.grassMaterial != null) grassManager.grassMaterial.SetMaterialToGrass();
            }
            else
            {
                EditorUtility.SetDirty(grassSettings);
                AssetsManager.SetGrassShaderSettings(true, grassSettings.useSimple, grassSettings.useFull);
            }
        }

        private void ReloadShadersAndSettings()
        {
            GrassSettings grassSettings = AssetsManager.GetGrassSettings();
            GrassManager grassManager = FindObjectOfType<GrassManager>();
            Shader.SetGlobalTexture("_GrassDepthTex", Texture2D.blackTexture);
            ResetShaderSettings(grassSettings, grassManager);
            
            AssetsManager.ResetGrassPhysicsSettings();
            AssetsManager.ReimportGrassPhysicsShaders();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Grass Physics/Grass Physics Settings")]
        public static void ShowWindow()
        {
            GrassWindow window = GetWindow<GrassWindow>("Grass Settings");
            string path = AssetsManager.GetGrassAssetPath() 
                + (EditorGUIUtility.isProSkin?"/Icons/GrassLightIco.png":"/Icons/GrassDarkIco.png");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            window.titleContent.image = texture;
            window.titleContent.text = "Grass Settings";
        }

        private Vector2 scrollPos;

        private void OnGUI()
        {
            //Set scroll position
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            AssetsManager.ResetGrassPhysicsSettings();

            EditorGUIUtility.labelWidth = 160;
            GrassManager grassManager;
            if (null == (grassManager = FindObjectOfType<GrassManager>()))
            {
                if (GUILayout.Button("Create grass manager"))
                {
                    GameObject instance = new GameObject("GrassManager", typeof(GrassManager));
                    Selection.activeGameObject = instance;
                }
            }

            GrassSettings grassSettings = AssetsManager.GetGrassSettings();

            if (grassSettings != null)
            {
                Undo.RecordObject(grassSettings, "Update in " + grassSettings.name);
                bool prevUseAuto = grassSettings.useAuto;
                grassSettings.useAuto = EditorGUILayout.Toggle("Use Auto Physics Mode", grassSettings.useAuto);

                if (grassSettings.useAuto)
                {
                    if (!prevUseAuto && grassManager)
                    {
                        EditorUtility.SetDirty(grassSettings);
                        AssetDatabase.SaveAssets();
                        grassManager.CorrectShaderSettings();

                    }
                    EditorGUILayout.HelpBox("With Auto Mode Enabled you cannot have different grass physics modes in the same game build.\n" +
                        "To use different physics modes for grass in multiple scenes, " +
                        "you have to disable it and set which physics modes you want to use.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Used physics modes:", EditorStyles.boldLabel);
                    grassSettings.useSimple = EditorGUILayout.Toggle("Simple Physics Mode", grassSettings.useSimple);
                    grassSettings.useFull = EditorGUILayout.Toggle("Full Physics Mode", grassSettings.useFull);
                    EditorUtility.SetDirty(grassSettings);
                    AssetsManager.SetGrassShaderSettings(true, grassSettings.useSimple, grassSettings.useFull);
                }
            }

            if (GUILayout.Button(new GUIContent("Reload settings and shaders",
                                                "Use this when you think that some shaders or settings changes haven't been applied")))
            {
                ReloadShadersAndSettings();
            }

            EditorGUILayout.EndScrollView();
        }

    }
}
