using System;
using UnityEditor;
using UnityEngine;
using UniWork.Utility.Editor;
using UniWork.Utility.Runtime;
using UniWork.Utility.Runtime.MethodUtility;

namespace UniWork.UIFramework.Editor.CodeGenerators
{
    internal static class UICtrlGenerator
    {
        private struct CtrlGenerateData
        {
            public string YourNamespace;
            public string PrefabName;
        }
        
        public static void GenerateCode(GameObject gameObject)
        {
            VerifySetting();
            CtrlGenerateData data = CollectGenerateData(gameObject);
            GenerateAndSaveCode(data);
        }
        
        private const string TemplatePath = "ScribanTemplates/UICtrlTemplate";

        private static void VerifySetting()
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            if (string.IsNullOrEmpty(editorSetting.codeFileRootPath))
                throw new Exception("[自动生成 UICtrl 代码]: 代码保存路径未设定");
        }
        
        private static CtrlGenerateData CollectGenerateData(GameObject selectedObject)
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();

            CtrlGenerateData data = new CtrlGenerateData
            {
                YourNamespace = $"{editorSetting.rootNamespace}.{selectedObject.name}",
                PrefabName = selectedObject.name
            };
            return data;
        }
        
        private static void GenerateAndSaveCode(CtrlGenerateData data)
        {
            string code = EditorMethodUtility.ScribanGenerateText(TemplatePath, data);

            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            string filePath = $"{editorSetting.codeFileRootPath}/{data.PrefabName}/{data.PrefabName}Ctrl.cs";
            
            IoUtility.OverlayWriteTextFile(filePath, code);
            
            AssetDatabase.Refresh();
            DLog.Info("[自动生成 UICtrl 代码]: 成功! " + filePath);
        }
    }
}