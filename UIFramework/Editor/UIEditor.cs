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
        public const string UIRootSavePath = "Assets/Resources/UIRoot.prefab";
        public const string UIRuntimeSettingSavePath = "Assets/Resources/UIRuntimeSetting.asset";
        
        [MenuItem("SFramework/UIFramework/创建UIRoot预设体")]
        public static void CreateUIRootPrefab()
        {
            if (File.Exists(UIRootSavePath))
                DLog.Warning("UIRoot预设体已存在：" + UIRootSavePath);
            
            string foldPath = UIRootSavePath.Substring(0, UIRootSavePath.LastIndexOf('/'));
            if (!Directory.Exists(foldPath))
                Directory.CreateDirectory(foldPath);

            GameObject uiRootObj = CreateUIRootObj();
            PrefabUtility.SaveAsPrefabAsset(uiRootObj, UIRootSavePath);
            
            GameObject.DestroyImmediate(uiRootObj);
            AssetDatabase.Refresh();

            Selection.activeObject = uiRootObj;
            EditorGUIUtility.PingObject(uiRootObj);
        }

        [MenuItem("SFramework/UIFramework/创建UITemplate模板")]
        public static void CreateUITemplate()
        {
            GameObject uiTemplate = new GameObject("UITemplate");
            uiTemplate.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = uiTemplate.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            CanvasScaler scaler = uiTemplate.AddComponent<CanvasScaler>();
            UIRuntimeSetting runtimeSetting = UIManager.Instance.RuntimeSetting;
            
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referenceResolution = new Vector2(runtimeSetting.width, runtimeSetting.height);
            scaler.matchWidthOrHeight = runtimeSetting.match;

            uiTemplate.AddComponent<GraphicRaycaster>();
        }

        [MenuItem("SFramework/UIFramework/创建UIRuntimeSetting")]
        public static void CreateUIRuntimeSetting()
        {
            if (File.Exists(UIRuntimeSettingSavePath))
                DLog.Warning("UIRuntimeSetting已存在：" + UIRuntimeSettingSavePath);
            
            string foldPath = UIRuntimeSettingSavePath.Substring(0, UIRuntimeSettingSavePath.LastIndexOf('/'));
            if (!Directory.Exists(foldPath))
                Directory.CreateDirectory(foldPath);
            
            UIRuntimeSetting runtimeSetting = ScriptableObject.CreateInstance<UIRuntimeSetting>();
            AssetDatabase.CreateAsset(runtimeSetting, UIRuntimeSettingSavePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = runtimeSetting;
            EditorGUIUtility.PingObject(runtimeSetting);
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
    }
}