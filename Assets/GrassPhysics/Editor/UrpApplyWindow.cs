using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

namespace ShadedTechnology.GrassPhysics
{
    public class UrpApplyWindow : EditorWindow
    {
        static ListRequest request;
        static readonly string templateUrpFilePath = "/URP/Shaders/Template/WavingGrassPasses.hlsl";
        static readonly string destUrpFilePath = "/Shaders/Terrain/WavingGrassPasses.hlsl";

        public static void CopyUrpTemplateFile(string destPath)
        {
            string urpFilePath = AssetsManager.GetGrassAssetPath() + templateUrpFilePath;
            if (File.Exists(urpFilePath) && File.Exists(destPath))
            {
                string text = File.ReadAllText(urpFilePath);
                text = text.Replace("{GrassAssetPath}", AssetsManager.GetGrassAssetPath());
                File.WriteAllText(destPath, text);
            }
        }

        public static void HandleRegistryURP(UnityEditor.PackageManager.PackageInfo package)
        {
            EditorGUILayout.HelpBox("Grass Physics shaders for URP are not applied!", MessageType.Warning);
            if (GUILayout.Button("Apply Grass Shaders to URP"))
            {
                string targetPath = "Packages/" + Path.GetFileName(package.resolvedPath);
                AssetsManager.CopyDirectory(package.resolvedPath, targetPath);
                string destFilePath = targetPath + destUrpFilePath;
                CopyUrpTemplateFile(destFilePath);
                AssetDatabase.Refresh();
            }
        }

        public static void HandleEmbeddedURP(UnityEditor.PackageManager.PackageInfo package)
        {
            EditorGUILayout.LabelField("URP Grass is applied.");
            if (GUILayout.Button("Reapply Grass Shaders to URP"))
            {
                string destFilePath = package.resolvedPath + destUrpFilePath;
                CopyUrpTemplateFile(destFilePath);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Reset URP"))
            {
                string targetPath = "Library/PackageCache/" + Path.GetFileName(package.resolvedPath);
                AssetsManager.CopyDirectory(package.resolvedPath, targetPath);
                Directory.Delete(package.resolvedPath, true);
                AssetDatabase.Refresh();
            }
        }

        public static void HandleURP()
        {
            if (request == null || request.Result == null)
            {
                request = Client.List();
                while (!request.IsCompleted) ;
            }
            foreach (UnityEditor.PackageManager.PackageInfo package in request.Result)
            {
                if (package.name.Contains("render-pipelines.universal"))
                {
                    EditorGUILayout.LabelField("URP Grass", EditorStyles.boldLabel);
                    if (package.source == PackageSource.Registry || package.source == PackageSource.BuiltIn)
                    {
                        HandleRegistryURP(package);
                    }
                    if (package.source == PackageSource.Embedded)
                    {
                        HandleEmbeddedURP(package);
                    }
                    EditorGUILayout.Space();
                }
            }
        }

        [MenuItem("Tools/Grass Physics/Grass URP Settings")]
        public static void ShowWindow()
        {
            UrpApplyWindow window = GetWindow<UrpApplyWindow>("Grass URP Settings");
            string path = AssetsManager.GetGrassAssetPath()
                + (EditorGUIUtility.isProSkin ? "/Icons/GrassLightIco.png" : "/Icons/GrassDarkIco.png");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            window.titleContent.image = texture;
            window.titleContent.text = "Grass URP Settings";
        }

        private void OnGUI()
        {
            HandleURP();
        }
    }
}