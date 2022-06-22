using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(XCameraController))]
    public class XCameraControllerEditor : Editor
    {
        private XCameraController _cameraController;


        private SerializedProperty FollowTarget;
        private SerializedProperty ViewRangeCollider2D;
        private SerializedProperty CameraOffset;
        
        private SerializedProperty ShakeDuration;
        private SerializedProperty ShakeDecay;
        private SerializedProperty ShakeIntensity;

        
        
        public  void OnEnable()
        {
            FollowTarget = serializedObject.FindProperty("FollowTarget");
            ViewRangeCollider2D = serializedObject.FindProperty("ViewRangeCollider2D");
            CameraOffset = serializedObject.FindProperty("CameraOffset");
            
            ShakeDuration = serializedObject.FindProperty("ShakeDuration");
            ShakeDecay = serializedObject.FindProperty("ShakeDecay");
            ShakeIntensity = serializedObject.FindProperty("ShakeIntensity");
        }

        public override void OnInspectorGUI()
        {
            _cameraController = target as XCameraController;
            if (_cameraController == null)
            {
                return;
            }
            EditorGUILayout.Space(10);
            EditorGUILayout.PrefixLabel("场景目标");
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(FollowTarget, new GUIContent("跟踪目标"),true);
            EditorGUILayout.PropertyField(ViewRangeCollider2D, new GUIContent("范围限制"),true);
            EditorGUILayout.PropertyField(CameraOffset, new GUIContent("摄像机偏移"),true);
            EditorGUILayout.LabelField("SrthographicSize",$"{Camera.main.orthographicSize}");
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.PrefixLabel("摄像机最大偏移设置");
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ManualUpDownLookDistance"), new GUIContent("摄像机上移下移深度"),true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CameraSpeed"), new GUIContent("摄像机平滑速度"),true);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.PrefixLabel("震动");
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(ShakeDuration, new GUIContent("持续时间"),true);
            EditorGUILayout.PropertyField(ShakeDecay, new GUIContent("时间衰减"),true);
            EditorGUILayout.PropertyField(ShakeIntensity, new GUIContent("强度"),true);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.PrefixLabel("偏移设置");
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LookAheadTrigger"), new GUIContent("偏移触发移动距离"),true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("HorizontalLookDistance"), new GUIContent("水平视角宽度"),true);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.PrefixLabel("缩放");
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ZoomInOutSpeed"), new GUIContent("摄像机缩放速度"),true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OpenMotionZoom"), new GUIContent("是否开启速度缩放平滑"),true);
            if (_cameraController.OpenMotionZoom)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MotionZoomSpeed"), new GUIContent("根据角色速度缩放平滑速度"),true);
                if (Camera.main.orthographic)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("当前摄像机模式:","[正交摄像机]");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ManualZoomInOutLookDistance"), new GUIContent("缩放深度"),true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MinimumZoom"), new GUIContent("最小视角"),true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MaximumZoom"), new GUIContent("最大视角"),true);
                }
                else
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("当前摄像机模式:","[透视摄像机]");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ManualZoomInOutLookAngle"), new GUIContent("缩放角度"),true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MinimumAngleZoom"),new GUIContent("最小缩放角度") ,true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MaximumAngleZoom"), new GUIContent("最大缩放角度"),true);
                }
            }
            EditorGUILayout.EndVertical();


            if (Application.isPlaying)
            {
                if (GUILayout.Button("摄像机向上移动"))
                {
                    _cameraController.LookUp();
                }
                else if (GUILayout.Button("摄像机向下移动"))
                {
                    _cameraController.LookDown();
                }
                else if(GUILayout.RepeatButton("摄像机移动重置"))
                {
                    _cameraController.ResetLookUpDown();
                }
                
                
                EditorGUILayout.Space(10);
                if (GUILayout.Button("摄像机缩进"))
                {
                    _cameraController.ZoomIn();
                }
                else if (GUILayout.Button("摄像机远离"))
                {
                    _cameraController.ZoomOut();
                }
                else if(GUILayout.RepeatButton("摄像机缩放重置"))
                {
                    _cameraController.ResetZoomInOut();
                }
                
                if (GUILayout.Button("震动"))
                {
                    _cameraController.Shake();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
