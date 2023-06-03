using System.IO;
using SFramework.UIFramework.Runtime;
using SFramework.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SFramework.UIFramework.Editor
{
    public class UIEditor
    {
        public const string UIRootSavePath = "Assets/Resources/UIRoot.prefab";
        
        [MenuItem("SFramework/UIFramework/创建UIRoot预设体")]
        public static void CreateUIRootPrefab()
        {
            if (File.Exists(UIRootSavePath))
                DLog.Warning("UIRoot预设体已存在：" + UIRootSavePath);

            GameObject uiRootObj = CreateUIRootObj();
            PrefabUtility.SaveAsPrefabAsset(uiRootObj, UIRootSavePath);
            
            GameObject.DestroyImmediate(uiRootObj);
            AssetDatabase.Refresh();
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