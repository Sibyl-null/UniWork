using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UniWork.UIFramework.Runtime.Scheduler;
using UniWork.Utility.Editor;

namespace UniWork.UIFramework.Editor
{
    public class UIEditorSetting : SerializedScriptableObject
    {
        [Serializable]
        public struct AutoBindData
        {
            public string prefix;
            public string componentName;

            public AutoBindData(string prefix, string componentName)
            {
                this.prefix = prefix;
                this.componentName = componentName;
            }
        }
        
        [Serializable]
        public struct InfoConfig
        {
            public string layerName;
            public UIScheduleMode scheduleMode;
            public string resPath;
        }

        public string codeFileSavePath = "Assets/Scripts/AutoGen/UI";
        public string codeNamespace = "AutoGen.UI";

        [Title("用于自动生成 UIView 代码")]
        public List<AutoBindData> autoBindComponents = new List<AutoBindData>()
        {
            new AutoBindData("txt", nameof(Text)),
            new AutoBindData("txt", nameof(TextMeshProUGUI)),
            new AutoBindData("btn", nameof(Button)),
            new AutoBindData("img", nameof(Image)),
            new AutoBindData("rawImg", nameof(RawImage)),
            new AutoBindData("trans", nameof(RectTransform)),
            new AutoBindData("go", nameof(GameObject)),
            new AutoBindData("sv", nameof(ScrollRect)),
            new AutoBindData("input", nameof(InputField)),
            new AutoBindData("cg", nameof(CanvasGroup)),
            new AutoBindData("dp", nameof(Dropdown))
        };

        [Title("用于自动生成 AddInfo 代码")]
        [DictionaryDrawerSettings(KeyLabel = "CtrlType", ValueLabel = "UIInfo")]
        public Dictionary<Type, InfoConfig> infoMap = new Dictionary<Type, InfoConfig>();
        
        // ----------------------------------------------------------------------------------

        private const string SavePath = "Assets/Editor/Config/UIEditorSetting.asset";

        public static UIEditorSetting MustLoad()
        {
            UIEditorSetting setting = AssetDatabase.LoadAssetAtPath<UIEditorSetting>(SavePath);
            if (setting == null)
                throw new Exception("UIEditorSetting 加载失败, path = " + SavePath);

            return setting;
        }

        public static void CreateAsset()
        {
            EditorMethodUtility.CreateScriptableObjectAsset<UIEditorSetting>(SavePath);
        }
    }
}