using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UniWork.UIFramework.Runtime;
using UniWork.Utility.Editor;
using UniWork.Utility.Runtime;
using UniWork.Utility.Runtime.MethodUtility;

namespace UniWork.UIFramework.Editor.CodeGenerators
{
    internal static class UIViewGenerator
    {
        private struct PropertyData
        {
            public readonly string TypeName;
            public readonly string PropertyName;

            public PropertyData(string typeName, string propertyName)
            {
                TypeName = typeName;
                PropertyName = propertyName;
            }
        }

        private class CodeGenerateData
        {
            public string YourNamespace;
            public string PrefabName;
            public string[] Namespaces;
            public List<PropertyData> Properties;
        }
        
        private const string TemplatePath = "ScribanTemplates/UIViewTemplate";
        
        public static void GenerateCode(UIComponentCollector collector)
        {
            VerifySetting();
            CodeGenerateData data = CollectGenerateData(collector);
            GenerateAndSaveCode(data);
        }

        private static void VerifySetting()
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            if (string.IsNullOrEmpty(editorSetting.codeFileRootPath))
                throw new Exception("[自动生成 UIView 代码]: 代码保存路径未设定");
        }

        private static CodeGenerateData CollectGenerateData(UIComponentCollector collector)
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            
            HashSet<string> namespaceSet = new HashSet<string> { typeof(UIBaseView).Namespace };
            List<PropertyData> propertyDataList = new List<PropertyData>();

            foreach (UIComponentCollector.ComponentInfo info in collector.ComponentInfos)
            {
                Type type = info.component.GetType();
                namespaceSet.Add(type.Namespace);
                propertyDataList.Add(new PropertyData(type.Name, info.propertyName));
            }

            CodeGenerateData data = new CodeGenerateData
            {
                YourNamespace = $"{editorSetting.rootNamespace}.{collector.name}",
                PrefabName = collector.name,
                Namespaces = namespaceSet.ToArray(),
                Properties = propertyDataList
            };
            return data;
        }

        private static void GenerateAndSaveCode(CodeGenerateData data)
        {
            string code = EditorMethodUtility.ScribanGenerateText(TemplatePath, data);

            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            string filePath = $"{editorSetting.codeFileRootPath}/{data.PrefabName}/{data.PrefabName}View.cs";
            
            IoUtility.OverlayWriteTextFile(filePath, code);
            
            AssetDatabase.Refresh();
            DLog.Info("[自动生成 UIView 代码]: 成功! " + filePath);
        }
    }
}