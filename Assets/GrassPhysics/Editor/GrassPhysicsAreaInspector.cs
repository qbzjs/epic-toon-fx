using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace ShadedTechnology.GrassPhysics
{
    [CustomEditor(typeof(GrassPhysicsArea))]
    public class GrassPhysicsAreaInspector : Editor
    {
        private void AddGrassPhysicsCamera()
        {
            GrassPhysicsCamera physicsCamera = myTarget.gameObject.AddComponent<GrassPhysicsCamera>();
            physicsCamera.physicsArea = myTarget;
            physicsCamera.Cam.cullingMask = 1;
            myTarget.AddOnSetDepthTextureEvent(physicsCamera.RenderTexture);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(physicsCamera.Cam);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(physicsCamera);
        }

        private void EventGUI()
        {
            SerializedProperty textureEvent = serializedObject.FindProperty("onSetDepthTextureEvent");
            if (textureEvent != null)
            {
                EditorGUILayout.PropertyField(textureEvent);
            }
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
        }

        private void UsePostProcessGUI()
        {
            EditorGUILayout.Space();

            serializedObject.Update();
            SerializedProperty postProcessProfile = serializedObject.FindProperty("postProcessProfile");
            if (postProcessProfile != null)
            {
                EditorGUILayout.PropertyField(postProcessProfile, new GUIContent("Grass Post Process Profile", 
                       "Set Post Process Profile to add special effects for grass physics such as trails in grass"));
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void FollowPlayerGUI()
        {
            myTarget.followPlayer = EditorGUILayout.Toggle("Follow Target", myTarget.followPlayer);
            if (myTarget.followPlayer)
            {
                EditorGUI.indentLevel++;
                myTarget.target = EditorGUILayout.ObjectField("Target Transform", myTarget.target, typeof(Transform), true) as Transform;
                if (myTarget.target)
                {
                    myTarget.areaOffset = EditorGUILayout.Vector3Field("Area Offset", myTarget.areaOffset);
                    myTarget.FollowTarget();
                }
                else
                {
                    EditorGUILayout.HelpBox("Assign target transform!", MessageType.Warning);
                }
                EditorGUI.indentLevel--;
            }
        }

        private void AreaAndTextureSizeGUI()
        {
            myTarget.areaSize.y = EditorGUILayout.FloatField("Area Height", myTarget.areaSize.y);
            myTarget.areaSize.z = EditorGUILayout.FloatField("Area Horizontal Size", myTarget.areaSize.z);
            if (null != myTarget.depthTexture)
            {
                myTarget.textureSize = new Vector2Int(myTarget.depthTexture.width, myTarget.depthTexture.height);
            }
            //Set areaSize.x basing on areaSize.z and texture width/height ratio
            myTarget.areaSize.x = ((float)myTarget.textureSize.x / (float)myTarget.textureSize.y) * myTarget.areaSize.z;
        }

        private void AddCameraButtonGUI()
        {
            if (myTarget.GetComponent<GrassPhysicsCamera>() == null)
            {
                if (GUILayout.Button("Add Grass Physics Camera"))
                {
                    AddGrassPhysicsCamera();
                }

                EditorGUILayout.Space();
            }
        }

        private void TextureGUI()
        {
            myTarget.depthTexture = EditorGUILayout.ObjectField("Depth Texture", myTarget.depthTexture, typeof(Texture), false) as Texture;
            if (myTarget.depthTexture == null)
            {
                EditorGUILayout.HelpBox("Depth texture is not assigned!", MessageType.Warning);
            }
        }

        private void ShowGUI()
        {
            AddCameraButtonGUI();
            AreaAndTextureSizeGUI();
            EditorGUILayout.Space();
            TextureGUI();
            EditorGUILayout.Space();
            EventGUI();
            EditorGUILayout.Space();
            FollowPlayerGUI();
            EditorGUILayout.Space();
            UsePostProcessGUI();
        }

        GrassPhysicsArea myTarget;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            myTarget = target as GrassPhysicsArea;
            Undo.RecordObject(myTarget, "Update in " + myTarget.name);

            EditorGUI.BeginChangeCheck();
            ShowGUI();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(myTarget);
            }
            serializedObject.ApplyModifiedProperties();
        }


        BoxBoundsHandle boxHandle = new BoxBoundsHandle();

        /// <summary>
        /// Draw Gizmos Handles
        /// </summary>
        protected virtual void OnSceneGUI()
        {
            if (myTarget == null) return;

            myTarget.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0));

            boxHandle.center = myTarget.transform.position + new Vector3(0, myTarget.areaSize.y / 2f, 0);
            boxHandle.size = myTarget.areaSize;

            EditorGUI.BeginChangeCheck();
            boxHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(myTarget, "Change Bounds");

                float newX = (myTarget.textureSize.x / myTarget.textureSize.y) * boxHandle.size.z;
                float newY = boxHandle.size.y;
                float newZ = boxHandle.size.z;
                if(newX > 0 && newY > 0 && newZ > 0)
                {
                    if(newZ != myTarget.areaSize.z)
                    {
                        myTarget.areaSize = new Vector3(((float)myTarget.textureSize.x / (float)myTarget.textureSize.y) * boxHandle.size.z,
                            boxHandle.size.y, boxHandle.size.z);
                    }
                    else
                    {
                        myTarget.areaSize = new Vector3(boxHandle.size.x,
                            boxHandle.size.y, ((float)myTarget.textureSize.y / (float)myTarget.textureSize.x) * boxHandle.size.x);
                    }

                    myTarget.areaOffset = new Vector3(myTarget.areaOffset.x,
                        (boxHandle.center.y - newY/2f) - myTarget.target.position.y,
                        myTarget.areaOffset.z);
                    if (myTarget.followPlayer)
                    {
                        myTarget.FollowTarget();
                    }
                }
            }
        }
    }
}