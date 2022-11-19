using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Class is responsible for managing grass physics modes and grass material
    /// </summary>
    [AddComponentMenu("Grass Physics/GrassManager")]
    public class GrassManager : MonoBehaviour {

        [SerializeField]
        public GrassPhysicsMode grassMode;

        [SerializeField]
        public GrassPhysics grassPhysics;

        [SerializeField]
        public GrassMaterialProfile grassMaterial;

        private void Start()
        {
            if (grassMaterial != null)
            {
                grassMaterial.SetMaterialToGrass();
            }
            if (grassMode == GrassPhysicsMode.None)
            {
                Shader.SetGlobalInt("_GrassPhysicsMode", (int)GrassPhysicsMode.None);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// True if grass has been reset
        /// </summary>
        bool hasResetGrass = false;
        private void OnDrawGizmos()
        {
            // Resets grass depth texture to black texture in edit mode
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (!hasResetGrass)
                {
                    if(grassMaterial != null) grassMaterial.SetMaterialToGrass();
                    Shader.SetGlobalTexture("_GrassDepthTex", Texture2D.blackTexture);
                    CorrectShaderSettings();
                    hasResetGrass = true;
                }
            }
            else
            {
                hasResetGrass = false;
            }
        }

        public bool IsPrefabOrMultipleManagers()
        {
#if UNITY_2019_2_OR_NEWER 
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(gameObject);
            return (!gameObject.activeInHierarchy || prefabType != PrefabAssetType.NotAPrefab || FindObjectsOfType<GrassManager>().Length > 1);
#else
            PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
            return (!gameObject.activeInHierarchy || prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.Prefab || FindObjectsOfType<GrassManager>().Length > 1);
#endif
        }

        /// <summary>
        /// Sets (global) <see cref="GrassMaterialProfile"/> settings if current <see cref="GrassManager"/> is not a prefab
        /// </summary>
        public void CorrectGrassMaterialSettings()
        {
            if (IsPrefabOrMultipleManagers()) return;

            string shaderPath = AssetsManager.GetGrassAssetPath() + "/Shaders/LegacyRP/WavingGrassPhysics.shader";
            System.Type materialType = null;
            if(grassMaterial != null)
            {
                materialType = grassMaterial.GetType();
            }
            AssetsManager.SetGrassFragmentInclude(GrassMaterialAttribute.GetFragmentShaderFromClassType(materialType));
            if (System.IO.File.Exists(shaderPath))
            {
                AssetsManager.SetSurfInShaderFile(shaderPath, GrassMaterialAttribute.GetSurfFromClassType(materialType));
            }
        }

        /// <summary>
        /// Sets (global) physics settings to correspond to what is in <see cref="grassProfile.grassMode"/>
        /// </summary>
        public void CorrectShaderSettings()
        {
            if (IsPrefabOrMultipleManagers()) return;

            GrassSettings settings = AssetsManager.GetGrassSettings();
            if (settings != null && settings.useAuto)
            {
                bool isSimple = (grassMode == GrassPhysicsMode.Simple);
                bool isFull = (grassMode == GrassPhysicsMode.Full);
                AssetsManager.SetGrassShaderSettings(false, isSimple, isFull);
            }
        }
#endif
        }
}
