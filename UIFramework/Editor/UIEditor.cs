using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniWork.UIFramework.Runtime;
using UniWork.Utility.Editor;
using UniWork.Utility.Runtime;
using Object = UnityEngine.Object;

namespace UniWork.UIFramework.Editor
{
    public static class UIEditor
    {
        // ---------------------------------------------------------------
        // 初始化资源
        // ---------------------------------------------------------------

        private const string UIRootDefaultSavePath = "Assets/Resources/UIRoot.prefab";
        private const string UIRuntimeSettingDefaultSavePath = "Assets/Resources/UIRuntimeSetting.asset";
        public const string AutoBindTag = "AutoField";

        [MenuItem("UniWork/UIFramework/创建全部", false, 1)]
        public static void CreateAll()
        {
            CreateUIRootPrefab();
            CreateUIEditorSetting();
            CreateUIRuntimeSetting();
            AddAutoFieldTag();
        }
        
        [MenuItem("UniWork/UIFramework/创建UIRoot预设体")]
        public static void CreateUIRootPrefab()
        {
            if (File.Exists(UIRootDefaultSavePath))
            {
                DLog.Warning("UIRoot预设体已存在：" + UIRootDefaultSavePath);
                return;
            }

            string foldPath = UIRootDefaultSavePath.Substring(0, UIRootDefaultSavePath.LastIndexOf('/'));
            if (!Directory.Exists(foldPath))
                Directory.CreateDirectory(foldPath);

            GameObject uiRootObj = CreateUIRootObj();
            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(uiRootObj, UIRootDefaultSavePath);
            Object.DestroyImmediate(uiRootObj);
            
            AssetDatabase.Refresh();
            Selection.activeObject = prefabAsset;
            EditorGUIUtility.PingObject(prefabAsset);
        }
        
        private static GameObject CreateUIRootObj()
        {
            GameObject uiRootObj = new GameObject("UIRoot");

            Transform eventSystemTrans = CreateEventSystemObj();
            eventSystemTrans.SetParent(uiRootObj.transform);

            Transform uiCameraTrans = CreateUICameraObj();
            uiCameraTrans.SetParent(uiRootObj.transform);

            ModifyUIRootObj(uiRootObj, uiCameraTrans.GetComponent<Camera>());
            
            return uiRootObj;
        }

        private static void ModifyUIRootObj(GameObject root, Camera uiCamera)
        {
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = uiCamera;

            CanvasScaler canvasScaler = root.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.matchWidthOrHeight = 1;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            
            root.layer = LayerMask.NameToLayer("UI");
        }

        private static Transform CreateUICameraObj()
        {
            GameObject uiCameraObj = new GameObject("UICamera");
            uiCameraObj.layer = LayerMask.NameToLayer("UI");
            uiCameraObj.transform.localPosition = new Vector3(0, 0, -100);
            
            Camera uiCamera = uiCameraObj.AddComponent<Camera>();
            uiCamera.orthographic = true;
            uiCamera.clearFlags = CameraClearFlags.Depth;
            uiCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            return uiCameraObj.transform;
        }

        private static Transform CreateEventSystemObj()
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            return eventSystemObj.transform;
        }

        [MenuItem("UniWork/UIFramework/创建UIEditorSetting")]
        public static void CreateUIEditorSetting()
        {
            UIEditorSetting.CreateAsset();
        }

        [MenuItem("UniWork/UIFramework/创建UIRuntimeSetting")]
        public static void CreateUIRuntimeSetting()
        {
            EditorMethodUtility.CreateScriptableObjectAsset<UIRuntimeSetting>(UIRuntimeSettingDefaultSavePath);
        }

        [MenuItem("UniWork/UIFramework/添加AutoField Tag")]
        public static void AddAutoFieldTag()
        {
            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset"));
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; ++i)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == AutoBindTag)
                {
                    DLog.Warning("已存在该Tag: " + AutoBindTag);
                    return;
                }
            }

            int index = tagsProp.arraySize;
            tagsProp.InsertArrayElementAtIndex(index);
            SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
            sp.stringValue = AutoBindTag;

            tagManager.ApplyModifiedProperties();
            DLog.Info("成功添加Tag: " + AutoBindTag);
        }
        
        [MenuItem("Assets/创建 UITemplate 模板", false, 100)]
        public static void CreateUITemplate()
        {
            string savePath = EditorMethodUtility.GetSelectedPath();
            if (savePath == "" || Directory.Exists(savePath) == false)
                throw new Exception("[CreateUITemplate]: 请选择一个文件夹");

            // create gameObject
            GameObject uiTemplate = new GameObject("ZTemplateUI");
            uiTemplate.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = uiTemplate.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            
            uiTemplate.AddComponent<GraphicRaycaster>();
            uiTemplate.AddComponent<UICodeGenerator>();

            GameObject contentObj = new GameObject("Content");
            RectTransform contentTrans = contentObj.AddComponent<RectTransform>();
            contentTrans.SetParent(uiTemplate.transform);
            contentTrans.offsetMin = Vector2.zero;
            contentTrans.offsetMax = Vector2.zero;
            contentTrans.anchorMin = Vector2.zero;
            contentTrans.anchorMax = Vector2.one;

            // save as prefab
            GameObject prefabAsset =
                PrefabUtility.SaveAsPrefabAsset(uiTemplate, Path.Combine(savePath, "ZTemplateUI.prefab"));
            Object.DestroyImmediate(uiTemplate);
            
            AssetDatabase.Refresh();
            Selection.activeObject = prefabAsset;
            EditorGUIUtility.PingObject(prefabAsset);
        }
    }
}