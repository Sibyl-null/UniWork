using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UniWork.Utility.Editor;
using UniWork.Utility.Runtime;

namespace UniWork.UIFramework.Editor.CodeGenerators
{
    internal static class UICtrlGenerator
    {
        private struct CtrlGenerateData
        {
            public string YourNamespace;
            public string CtrlClassName;
            public string ViewClassName;
        }
        
        [MenuItem("Assets/自动生成UICtrl代码", false, UIEditor.MenuItemPriority)]
        public static void GenerateCtrlCode()
        {
            GameObject selectedObject = VerifySetting();
            CtrlGenerateData data = CollectGenerateData(selectedObject);
            GenerateAndSaveCode(data);
        }

        private static GameObject VerifySetting()
        {
            string selectedPath = EditorMethodUtility.GetSelectedPath();
            if (selectedPath == "" || File.Exists(selectedPath) == false)
                throw new Exception("[自动生成UICtrl代码]: 请选择一个Prefab");

            GameObject selectedObject = AssetDatabase.LoadAssetAtPath<GameObject>(selectedPath);
            if (selectedObject == null)
                throw new Exception("[自动生成UICtrl代码]: 请选择一个Prefab");

            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            if (string.IsNullOrEmpty(editorSetting.codeFileSavePath))
                throw new Exception("[自动生成UICtrl代码]: 代码保存路径未设定");

            return selectedObject;
        }
        
        private static CtrlGenerateData CollectGenerateData(GameObject selectedObject)
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();

            CtrlGenerateData data = new CtrlGenerateData
            {
                YourNamespace = editorSetting.rootNamespace,
                CtrlClassName = $"{selectedObject.name}Ctrl",
                ViewClassName = $"{selectedObject.name}View"
            };
            return data;
        }
        
        private static void GenerateAndSaveCode(CtrlGenerateData data)
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            string code = EditorMethodUtility.ScribanGenerateText("UICtrlTemplate", data);

            string savePath = Path.Combine(editorSetting.codeFileSavePath, $"{data.CtrlClassName}.cs");
            if (Directory.Exists(editorSetting.codeFileSavePath) == false)
                Directory.CreateDirectory(editorSetting.codeFileSavePath);
            
            File.WriteAllText(savePath, code);
            
            AssetDatabase.Refresh();
            DLog.Info("[自动生成UICtrl代码]: 成功! " + savePath);
        }
    }
}