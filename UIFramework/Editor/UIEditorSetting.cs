using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UniWork.UIFramework.Editor.CodeGenerators;
using UniWork.Utility.Editor;

namespace UniWork.UIFramework.Editor
{
    public class UIEditorSetting : ScriptableObject
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
        
        [TitleGroup("代码生成通用配置"), FolderPath]
        public string codeFileRootPath = "Assets/Scripts/UI";
        
        [TitleGroup("代码生成通用配置")]
        public string rootNamespace = "UI";
        
        [TitleGroup("UIConfig 生成配置")]
        public bool resPathWithExtension = true;
        
        [TitleGroup("UIConfig 生成配置")]
        public string resPathRemovePrefix;
        
        [TitleGroup("UIConfig 生成配置")]
        public List<DefaultAsset> prefabSearchFolders = new();
        
        [TitleGroup("View 组件自动绑定配置"), TableList]
        public List<AutoBindData> autoBindComponents = new List<AutoBindData>()
        {
            new AutoBindData("Txt", nameof(Text)),
            new AutoBindData("Tmp", nameof(TextMeshProUGUI)),
            new AutoBindData("Btn", nameof(Button)),
            new AutoBindData("Img", nameof(Image)),
            new AutoBindData("RawImg", nameof(RawImage)),
            new AutoBindData("Trans", nameof(RectTransform)),
            new AutoBindData("Go", nameof(GameObject)),
            new AutoBindData("Sv", nameof(ScrollRect)),
            new AutoBindData("Input", nameof(InputField)),
            new AutoBindData("Cg", nameof(CanvasGroup)),
            new AutoBindData("Dp", nameof(Dropdown))
        };
        
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

        [TitleGroup("UIConfig 生成配置"), Button("生成 UIConfig 代码")]
        private void GenerateUIConfig()
        {
            UIConfigGenerator.GenerateCode();
        }
    }
}