using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ShadedTechnology.GrassPhysics
{
    public class PhysicsMaterialEditor : ShaderGUI
    {
        enum RenderQueue { Geometry = 2000, AlphaTest = 2450, Transparent = 3000 }

        private int tabMode;
        private static readonly string[] tabNames = { "Only Wind", "Simple Physics", "Full Physics" };
        private static readonly string[] tabValues = { "", "PHYSICS_SIMPLE", "PHYSICS_FULL" };

        private static readonly string[] fullPhysicsValues = { "_GrassPhysicsAreaPos", "_GrassPhysicsAreaSize", "_GrassPhysicsOffset", "_GrassTexEnlargement", "_GrassDepthTex" };
        private static readonly string[] simplePhysicsValues = { "_CanDeformUp", "_DisplacementLimits", "_CanTilt" };

        private bool debugMode = false;

        private RenderQueue renderQueue;

        private void CorrectValues(string[] keyWords)
        {
            debugMode = false;
            for (int i = 0; i < tabValues.Length; ++i)
            {
                if (keyWords.Contains(tabValues[i]))
                {
                    tabMode = i;
                    return;
                }
                if (keyWords.Contains("DEBUG_GRASS_WIND"))
                {
                    debugMode = true;
                }
            }
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        private void DisplayStandardUnityMaterialOptions(Material targetMat)
        {
            EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 200;
            GUILayout.BeginHorizontal();
            renderQueue = (RenderQueue)targetMat.renderQueue;
            renderQueue = (RenderQueue)EditorGUILayout.EnumPopup("Render Queue", renderQueue);
            targetMat.renderQueue = (int)renderQueue;
            targetMat.renderQueue = EditorGUILayout.IntField(targetMat.renderQueue, GUILayout.MaxWidth(80));
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 100;
            targetMat.enableInstancing = EditorGUILayout.Toggle("Enable GPU Instancing", targetMat.enableInstancing);
            targetMat.doubleSidedGI = EditorGUILayout.Toggle("Double Sided Globla Illumination", targetMat.doubleSidedGI);
        }

        private void CheckForSpace(MaterialProperty property)
        {
            if ((property.flags & MaterialProperty.PropFlags.HideInInspector) > 0)
            {
                EditorGUILayout.Space();
            }
        }

        private void DisplayStandardProperties(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            foreach (MaterialProperty property in properties)
            {
                if ((property.flags & MaterialProperty.PropFlags.HideInInspector) > 0) continue;
                CheckForSpace(property);
                string propertyName = property.name;
                if (!fullPhysicsValues.Contains(propertyName) && !simplePhysicsValues.Contains(propertyName))
                {
                    materialEditor.ShaderProperty(property, property.displayName);
                }
            }
        }

        private void DisplayModeSpecificProperties(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            foreach (MaterialProperty property in properties)
            {
                CheckForSpace(property);
                string propertyName = property.name;
                if (2 == tabMode && (fullPhysicsValues.Contains(propertyName)) || (1 == tabMode && simplePhysicsValues.Contains(propertyName)))
                {
                    materialEditor.ShaderProperty(property, property.displayName);
                }
            }
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material targetMat = materialEditor.target as Material;
            string[] Keywords = targetMat.shaderKeywords;
            CorrectValues(Keywords);

            DisplayStandardProperties(materialEditor, properties);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            debugMode = GUILayout.Toggle(debugMode, "Debug wind");

            tabMode = GUILayout.Toolbar(tabMode, tabNames);
            DisplayModeSpecificProperties(materialEditor, properties);
            EditorGUILayout.Space();

            DrawUILine(new Color(0.16f, 0.16f, 0.16f));
            DisplayStandardUnityMaterialOptions(targetMat);

            // If something has changed, update the material.
            if (EditorGUI.EndChangeCheck())
            {
                targetMat.shaderKeywords = new string[] { tabValues[tabMode], debugMode ? "DEBUG_GRASS_WIND" : "NO_DEBUG" };
                EditorUtility.SetDirty(targetMat);
            }
        }
    }
}
