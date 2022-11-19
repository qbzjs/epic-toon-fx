using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ShadedTechnology.GrassPhysics
{
    [CustomEditor(typeof(GrassPhysicsCamera))]
    public class GrassPhysicsCameraInspector : Editor
    {
        /// <summary>
        /// Returns true if terrain's layer is in culling mask
        /// </summary>
        /// <param name="cullingMask">Used culling mask</param>
        /// <returns>True if terrain's layer is in culling mask</returns>
        private bool IsTerrainInCullingMask(int cullingMask)
        {
            Terrain terrain = FindObjectOfType<Terrain>();
            return (terrain && ((1 << terrain.gameObject.layer) & cullingMask) != 0);
        }

        private void CheckIfEventIsAssignedAndAssignIfNot()
        {
            for(int i = 0; i < myTarget.physicsArea.onSetDepthTextureEvent.GetPersistentEventCount(); ++i)
            {
                if (myTarget.physicsArea.onSetDepthTextureEvent.GetPersistentMethodName(i).Equals("RenderTexture"))
                {
                    return;
                }
            }

            myTarget.physicsArea.AddOnSetDepthTextureEvent(myTarget.RenderTexture);
        }

        private void SetTextureResolution()
        {
            UnityEngine.Vector2Int texSize = EditorGUILayout.Vector2IntField("Texture Resolution", myTarget.textureSize);
            myTarget.textureSize = new UnityEngine.Vector2Int(Mathf.Max(texSize.x - (texSize.x % GlobalConstants.STEP_PX), GlobalConstants.STEP_PX),
                                                  Mathf.Max(texSize.y - (texSize.y % GlobalConstants.STEP_PX), GlobalConstants.STEP_PX));
        }
        
        private void ShowGUI()
        {
            myTarget.Cam.cullingMask = EditorHelper.LayerMaskField("Culling Mask", myTarget.Cam.cullingMask);
            if (IsTerrainInCullingMask(myTarget.Cam.cullingMask))
            {
                EditorGUILayout.HelpBox("Terrain is in culling mask, uncheck terrain's layer from culling mask or change terrain's layer!", MessageType.Warning);
            }

            EditorGUILayout.Space();

            myTarget.physicsArea = EditorGUILayout.ObjectField("Physics Area", myTarget.physicsArea, typeof(GrassPhysicsArea), true) as GrassPhysicsArea;
            if(myTarget.physicsArea != null)
            {
                SetTextureResolution();
                myTarget.SetCameraSettings();
                if (myTarget.Cam.targetTexture == null || !myTarget.IsGoodTextureSize())
                {
                    myTarget.SetCameraTargetTexture();
                }
                else
                {
                    myTarget.physicsArea.depthTexture = myTarget.Cam.targetTexture;
                }
                CheckIfEventIsAssignedAndAssignIfNot();
            }
        }

        GrassPhysicsCamera myTarget;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            myTarget = target as GrassPhysicsCamera;
            Undo.RecordObject(myTarget, "Update in " + myTarget.name);
            Undo.RecordObject(myTarget.Cam, "Update in " + myTarget.Cam.name);

            EditorGUI.BeginChangeCheck();
            ShowGUI();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(myTarget);
                EditorUtility.SetDirty(myTarget.Cam);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
