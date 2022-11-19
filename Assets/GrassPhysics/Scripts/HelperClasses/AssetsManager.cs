#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Helper class for managing grass asset files
    /// </summary>
    public class AssetsManager : Editor
    {
        /// <summary>
        /// Creates path relative to the project folder from absolute path
        /// </summary>
        /// <param name="absolutePath">Absolute path</param>
        /// <returns>Path relative to the project folder</returns>
        public static string AbsolutePathToRelative(string absolutePath)
        {
            return absolutePath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
        }

        /// <summary>
        /// Checks if target platform is low end (different than Windows, XboxOne or PS4)
        /// </summary>
        /// <returns>True if target platform is low end</returns>
        public static bool IsLowEndPlatform()
        {
            return (!EditorUserBuildSettings.activeBuildTarget.Equals(BuildTarget.StandaloneWindows) &&
                    !EditorUserBuildSettings.activeBuildTarget.Equals(BuildTarget.StandaloneWindows64) &&
                    !EditorUserBuildSettings.activeBuildTarget.Equals(BuildTarget.XboxOne) &&
                    !EditorUserBuildSettings.activeBuildTarget.Equals(BuildTarget.PS4));
        }

        /// <summary>
        /// Returns list of Types that derives from given <see cref="Type"/>
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static List<Type> GetListOfType(Type baseType)
        {
            List<Type> objects = new List<Type>();
            foreach (Type type in
                Assembly.GetAssembly(baseType).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(baseType)))
            {
                objects.Add(type);
            }
            return objects;
        }

        /// <summary>
        /// Returns path to root grass asset folder
        /// </summary>
        /// <returns>Path to root grass asset folder</returns>
        public static string GetGrassAssetPath()
        {
            AssetsManager script = ScriptableObject.CreateInstance<AssetsManager>();
            MonoScript ms = MonoScript.FromScriptableObject(script);
            string filePath = AssetDatabase.GetAssetPath(ms);
            DestroyImmediate(script);
            DirectoryInfo dir = Directory.GetParent(filePath).Parent.Parent;
            string folderPath = AbsolutePathToRelative(dir.ToString());
            return folderPath;
        }

        /// <summary>
        /// Pop ups <see cref="EditorUtility.SaveFilePanel"/> opened in given directory, 
        /// then if chosen path is correct (selection wasn't canceled)
        /// <see cref="ScriptableObject"/> is created and saved in chosen path
        /// and then returned
        /// </summary>
        /// <param name="defaultPath">Path where <see cref="EditorUtility.SaveFilePanel"/> opens</param>
        /// <returns>Created <see cref="ScriptableObject"/> asset</returns>
        public static T CreateNewScriptableObjectOfType<T>(string title, string directory, string defaultName, string extension) where T : ScriptableObject
        {
            string path = EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
            path = AssetsManager.AbsolutePathToRelative(path);
            if (!string.IsNullOrEmpty(path))
            {
                T profile = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(profile, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return profile;
            }
            return null;
        }

        /// <summary>
        /// Pop ups <see cref="EditorUtility.SaveFilePanel"/> opened in given directory, 
        /// then if chosen path is correct (selection wasn't canceled)
        /// <see cref="ScriptableObject"/> is created and saved in chosen path
        /// and then returned
        /// </summary>
        /// <param name="defaultPath">Path where <see cref="EditorUtility.SaveFilePanel"/> opens</param>
        /// <returns>Created <see cref="ScriptableObject"/> asset</returns>
        public static ScriptableObject CreateNewScriptableObjectOfType(Type type, string title, string directory, string defaultName, string extension)
        {
            string path = EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
            path = AssetsManager.AbsolutePathToRelative(path);
            if (!string.IsNullOrEmpty(path))
            {
                ScriptableObject profile = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(profile, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return profile;
            }
            return null;
        }

        /// <summary>
        /// Gets scene path of current <see cref="SerializedProperty"/>
        /// </summary>
        /// <param name="property"><see cref="SerializedProperty"/> with scene path we are searching for</param>
        /// <returns>Scene path relative to the project directory in a <see cref="string"/></returns>
        public static string GetScenePath(SerializedProperty property)
        {
            string scenePath = (property.serializedObject.targetObject as MonoBehaviour).gameObject.scene.path;
            scenePath = Path.GetDirectoryName(scenePath);
            if (string.IsNullOrEmpty(scenePath))
            {
                scenePath = "Assets/";
            }
            return scenePath;
        }

        /// <summary>
        /// Returns <see cref="GrassSettings"/> scriptable object from "Settings" folder
        /// </summary>
        /// <returns>Grass mode settings</returns>
        public static GrassSettings GetGrassSettings()
        {
            string settingsPath = AssetsManager.GetGrassAssetPath() + "/Settings/GrassSettings.asset";
            return AssetsManager.LoadOrCreateObject(settingsPath, typeof(GrassSettings)) as GrassSettings;
        }

        /// <summary>
        /// Sets physics modes for global grass shader writing to GrassSettings.cginc file
        /// </summary>
        /// <param name="multiMode">Enable Multi Physics Mode</param>
        /// <param name="simpleMode">Enable Simple Physics Mode</param>
        /// <param name="fullMode">Enable Full Physics Mode</param>
        public static void SetGrassShaderSettings(bool multiMode, bool simpleMode, bool fullMode)
        {
            string content = "";
            if (multiMode) content += "#define PHYSICS_MULTI 1\n";
            if (simpleMode) content += "#define PHYSICS_SIMPLE 1\n";
            if (fullMode) content += "#define PHYSICS_FULL 1\n";

            string filePath = GetGrassAssetPath() + "/Shaders/Core/GrassSettings.cginc";
            if(!CheckIfFileAndStringAreTheSame(filePath, content))
            {
                AssetDatabase.SaveAssets();
                WriteStringToFile(filePath, content);
            }
        }

        /// <summary>
        /// Resets defined settings in grass physics shaders to use global constants from GlobalConstants class
        /// </summary>
        public static void ResetGrassPhysicsSettings()
        {
            string content = "#define PHYSICS_SIMPLE_MAX_COUNT " + GlobalConstants.MAX_GRASS_ACTORS;
            string filePath = GetGrassAssetPath() + "/Shaders/Core/GrassPhysicsSettings.cginc";
            if (!CheckIfFileAndStringAreTheSame(filePath, content))
            {
                WriteStringToFile(filePath, content);
                AssetDatabase.Refresh();
            }
        }


        /// <summary>
        /// Import recursively assets at the specified path
        /// </summary>
        /// <param name="path">Path to file or directory to import</param>
        public static void RecursivelyImportAsset(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    RecursivelyImportAsset(f);
                }
                foreach (string d in Directory.GetDirectories(path))
                {
                    RecursivelyImportAsset(d);
                }
            }
            else if (File.Exists(path))
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
            }
        }

        /// <summary>
        /// Reimports grass physics shaders
        /// </summary>
        public static void ReimportGrassPhysicsShaders()
        {
            string folderPath = GetGrassAssetPath() + "/Shaders/";
            RecursivelyImportAsset(folderPath);

            string urpPath = GetGrassAssetPath() + "/URP/";
            if (Directory.Exists(urpPath))
            {
                string urpShadersPath = urpPath + "Shaders/";
                RecursivelyImportAsset(urpShadersPath);
                string urpShaderGraphPath = urpPath + "ShaderGraph/";
                RecursivelyImportAsset(urpShaderGraphPath);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Loads <see cref="ScriptableObject"/> at path or creates it if doesn't exists
        /// </summary>
        /// <param name="path">Path to object</param>
        /// <param name="type">Type of object</param>
        /// <returns>Loaded <see cref="ScriptableObject"/></returns>
        public static ScriptableObject LoadOrCreateObject(string path, System.Type type)
        {
            ScriptableObject result;
            result = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject)) as ScriptableObject;
            if (result == null)
            {
                result = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(result, path);
                AssetDatabase.SaveAssets();
            }
            return result;
        }

        /// <summary>
        /// Writes string to file
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="textToWrite">String to write</param>
        public static void WriteStringToFile(string filePath, string textToWrite)
        {
            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(textToWrite);
            writer.Close();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Finds and replaces strings in file
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="oldStrings">Strings to replace</param>
        /// <param name="newStrings">Replacement strings</param>
        public static void FindAndReplaceInFile(string filePath, string[] oldStrings, string[] newStrings)
        {
            string text = File.ReadAllText(filePath);
            int len = oldStrings.Length > newStrings.Length ? newStrings.Length : oldStrings.Length;
            for(int i = 0; i < len; ++i)
            {
                text = text.Replace(oldStrings[i], newStrings[i]);
            }
            File.WriteAllText(filePath, text);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Finds first surf type in shader and replaces with given surf if needed
        /// </summary>
        /// <param name="filePath">Path to shader file</param>
        /// <param name="surf">Surf type name</param>
        public static void SetSurfInShaderFile(string filePath, string surf)
        {
            string text = File.ReadAllText(filePath);
            const string surfSubstr = "#pragma surface surf";
            if (text.Contains(surfSubstr))
            {
                int index = text.IndexOf(surfSubstr, 0);
                index += surfSubstr.Length;
                while (' ' == text[++index]);
                int count = text.IndexOf(" ", index) - index;
                
                if (surf.Equals(text.Substring(index, count))) return;

                text = text.Remove(index, count);
                text = text.Insert(index, surf);
            }
            File.WriteAllText(filePath, text);
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
        }

        /// <summary>
        /// Includes given fragment shader to grass shader 
        /// </summary>
        /// <param name="fragmentPath">Path to fragment shader include</param>
        public static void SetGrassFragmentInclude(string fragmentPath)
        {
            string includeFilePath = GetGrassAssetPath() + "/Shaders/Core/GrassSurface.cginc";
            string includeString = "#include \"" + fragmentPath + "\"";
            string text = File.ReadAllText(includeFilePath);
            if (text.Equals(includeString)) return;
            File.WriteAllText(includeFilePath, includeString);
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(includeFilePath, ImportAssetOptions.ForceUpdate);
        }

        /// <summary>
        /// Returns true if file contains string
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="text">String to check</param>
        /// <returns>True if file contains</returns>
        public static bool CheckIfFileContains(string filePath, string text)
        {
            StreamReader reader = new StreamReader(filePath);

            bool contains = reader.ReadToEnd().Contains(text);

            reader.Close();

            return contains;
        }


        /// <summary>
        /// Returns true if file and string are the same
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="text">String to compare</param>
        /// <returns>True if file content is the same as string</returns>
        public static bool CheckIfFileAndStringAreTheSame(string filePath, string textToCompare)
        {
            StreamReader reader = new StreamReader(filePath);

            bool same = (reader.ReadToEnd().CompareTo(textToCompare) == 0);

            reader.Close();

            return same;
        }

        public static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyDirectory(diSource, diTarget);
        }

        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
#endif