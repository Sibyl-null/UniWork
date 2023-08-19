using System.IO;
using SFramework.UIFramework.Runtime;
using SFramework.Utility.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SFramework.UIFramework.Editor
{
    public class UIEditor
    {
        // ---------------------------------------------------------------
        // 初始化资源
        // ---------------------------------------------------------------
        
        public const string UIRootDefaultSavePath = "Assets/Resources/UIRoot.prefab";
        public const string UIRuntimeSettingDefaultSavePath = "Assets/Resources/UIRuntimeSetting.asset";
        public const string UIEditorSettingDefaultSavePath = "Assets/Editor/Config/UIEditorSetting.asset";
        public const string AutoBindTag = "AutoField";

        [MenuItem("SFramework/UIFramework/初始化/创建全部", false, 1)]
        public static void CreateAll()
        {
            CreateUIRootPrefab();
            CreateUIRuntimeSetting();
            CreateUIEditorSetting();
            AddAutoFieldTag();
        }
        
        [MenuItem("SFramework/UIFramework/初始化/创建UIRoot预设体")]
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
            GameObject.DestroyImmediate(uiRootObj);
            
            AssetDatabase.Refresh();
            Selection.activeObject = prefabAsset;
            EditorGUIUtility.PingObject(prefabAsset);
        }
        
        private static GameObject CreateUIRootObj()
        {
            GameObject uiRootObj = new GameObject("UIRoot");
            uiRootObj.AddComponent<UIManager>();

            Transform eventSystemTrans = CreateEventSystemObj();
            eventSystemTrans.SetParent(uiRootObj.transform);

            Transform uiCameraTrans = CreateUICameraObj();
            uiCameraTrans.SetParent(uiRootObj.transform);

            return uiRootObj;
        }

        private static Transform CreateUICameraObj()
        {
            GameObject uiCameraObj = new GameObject("UICamera");
            uiCameraObj.layer = LayerMask.NameToLayer("UI");
            Camera uiCamera = uiCameraObj.AddComponent<Camera>();
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

        [MenuItem("SFramework/UIFramework/初始化/创建UIRuntimeSetting")]
        public static void CreateUIRuntimeSetting()
        {
            if (File.Exists(UIRuntimeSettingDefaultSavePath))
            {
                DLog.Warning("UIRuntimeSetting已存在：" + UIRuntimeSettingDefaultSavePath);
                return;
            }

            string foldPath =
                UIRuntimeSettingDefaultSavePath.Substring(0, UIRuntimeSettingDefaultSavePath.LastIndexOf('/'));
            if (!Directory.Exists(foldPath))
                Directory.CreateDirectory(foldPath);
            
            UIRuntimeSetting runtimeSetting = ScriptableObject.CreateInstance<UIRuntimeSetting>();
            AssetDatabase.CreateAsset(runtimeSetting, UIRuntimeSettingDefaultSavePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = runtimeSetting;
            EditorGUIUtility.PingObject(runtimeSetting);
        }

        [MenuItem("SFramework/UIFramework/初始化/创建UIEditorSetting")]
        public static void CreateUIEditorSetting()
        {
            if (File.Exists(UIEditorSettingDefaultSavePath))
            {
                DLog.Warning("UIEditorSetting已存在：" + UIEditorSettingDefaultSavePath);
                return;
            }

            string foldPath =
                UIEditorSettingDefaultSavePath.Substring(0, UIEditorSettingDefaultSavePath.LastIndexOf('/'));
            if (!Directory.Exists(foldPath))
                Directory.CreateDirectory(foldPath);
            
            UIEditorSetting editorSetting = ScriptableObject.CreateInstance<UIEditorSetting>();
            AssetDatabase.CreateAsset(editorSetting, UIEditorSettingDefaultSavePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = editorSetting;
            EditorGUIUtility.PingObject(editorSetting);
        }

        [MenuItem("SFramework/UIFramework/初始化/添加AutoField Tag")]
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
        
        
        // ---------------------------------------------------------------
        // 工作流
        // ---------------------------------------------------------------

        private const int MenuItemPriority = 100;
        
        [MenuItem("Assets/创建UITemplate模板", false, MenuItemPriority)]
        public static void CreateUITemplate()
        {
            string savePath = GetSelectedPath();
            if (savePath == "" || Directory.Exists(savePath) == false)
            {
                DLog.Warning("[CreateUITemplate]: 请选择一个文件夹");
                return;
            }
            
            // create gameObject
            GameObject uiTemplate = new GameObject("ZTemplateUI");
            uiTemplate.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = uiTemplate.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            CanvasScaler scaler = uiTemplate.AddComponent<CanvasScaler>();
            UIRuntimeSetting runtimeSetting =
                AssetDatabase.LoadAssetAtPath<UIRuntimeSetting>(UIRuntimeSettingDefaultSavePath);
            
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referenceResolution = new Vector2(runtimeSetting.width, runtimeSetting.height);
            scaler.matchWidthOrHeight = runtimeSetting.match;
            
            uiTemplate.AddComponent<GraphicRaycaster>();

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
            GameObject.DestroyImmediate(uiTemplate);
            
            AssetDatabase.Refresh();
            Selection.activeObject = prefabAsset;
            EditorGUIUtility.PingObject(prefabAsset);
        }

        private static string GetSelectedPath()
        {
            string selectedPath = "";
            
            if (Selection.assetGUIDs.Length > 0)
                selectedPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            
            return selectedPath;
        }
        
        [MenuItem("Assets/自动生成UIView代码", false, MenuItemPriority)]
        public static void GenerateUIViewCode()
        {
            string selectedPath = GetSelectedPath();
            if (selectedPath == "" || File.Exists(selectedPath) == false)
            {
                DLog.Warning("[自动生成UIView代码]: 请选择一个Prefab");
                return;
            }

            GameObject selectedObject = AssetDatabase.LoadAssetAtPath<GameObject>(selectedPath);
            if (selectedObject == null)
            {
                DLog.Warning("[自动生成UIView代码]: 请选择一个Prefab");
                return;
            }
            
            
        }
    }
}