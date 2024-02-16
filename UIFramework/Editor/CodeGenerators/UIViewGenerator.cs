using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UniWork.UIFramework.Runtime;
using UniWork.Utility.Editor;
using UniWork.Utility.Runtime;
using UniWork.Utility.Runtime.MethodUtility;

namespace UniWork.UIFramework.Editor.CodeGenerators
{
    internal static class UIViewGenerator
    {
        private struct FieldData
        {
            public readonly string TypeName;
            public readonly string FieldName;
            public readonly string GoName;

            public FieldData(string typeName, string fieldName, string goName)
            {
                TypeName = typeName;
                FieldName = fieldName;
                GoName = goName;
            }
        }

        private class CodeGenerateData
        {
            public string YourNamespace;
            public string PrefabName;
            public string[] Namespaces;
            public Dictionary<string, string> GoNamePathMap;
            public List<FieldData> Fields;
        }
        
        private const string TemplatePath = "ScribanTemplates/UIViewTemplate";
        
        public static void GenerateCode(GameObject gameObject)
        {
            VerifySetting();
            CodeGenerateData data = CollectGenerateData(gameObject);
            GenerateAndSaveCode(data);
        }

        private static void VerifySetting()
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            if (string.IsNullOrEmpty(editorSetting.codeFileRootPath))
                throw new Exception("[自动生成 UIView 代码]: 代码保存路径未设定");
        }

        private static CodeGenerateData CollectGenerateData(GameObject selectedObject)
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            
            HashSet<string> namespaceSet = new HashSet<string> { typeof(UIComponentCollector).Namespace };
            Dictionary<string, string> goNamePathMap = new Dictionary<string, string>();
            List<FieldData> fieldList = new List<FieldData>();

            List<GameObject> childList = new List<GameObject>();
            GetAllChildGameObjects(selectedObject.transform, ref childList);

            var gameObjectBindData =
                editorSetting.autoBindComponents.Where(bindData => bindData.componentName == nameof(GameObject))
                    .ToArray();
            bool shouldBindGameObject = gameObjectBindData.Length > 0;

            foreach (GameObject childObj in childList)
            {
                if (childObj.CompareTag(UIEditor.AutoBindTag) == false)
                    continue;

                if (goNamePathMap.ContainsKey(childObj.name))
                    throw new Exception("[自动生成 UIView 代码]: 存在对象重名 " + childObj.name);
                goNamePathMap.Add(childObj.name, GetChildFindPath(childObj.transform));

                Component[] components = childObj.GetComponents<Component>();
                foreach (Component component in components)
                {
                    foreach (var autoBindData in editorSetting.autoBindComponents
                                 .Where(bindData => bindData.componentName == component.GetType().Name))
                    {
                        namespaceSet.Add(component.GetType().Namespace);
                        fieldList.Add(new FieldData(autoBindData.componentName, autoBindData.prefix + childObj.name,
                            childObj.name));
                        break;
                    }
                }

                if (shouldBindGameObject)
                {
                    namespaceSet.Add(typeof(GameObject).Namespace);
                    fieldList.Add(new FieldData(nameof(GameObject), gameObjectBindData[0].prefix + childObj.name,
                        childObj.name));
                }
            }

            CodeGenerateData data = new CodeGenerateData
            {
                YourNamespace = $"{editorSetting.rootNamespace}.{selectedObject.name}",
                PrefabName = selectedObject.name,
                Namespaces = namespaceSet.ToArray(),
                GoNamePathMap = goNamePathMap,
                Fields = fieldList
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

        private static void GetAllChildGameObjects(Transform parent, ref List<GameObject> result)
        {
            result.Add(parent.gameObject);
            foreach (Transform childTrans in parent)
                GetAllChildGameObjects(childTrans, ref result);
        }

        private static string GetChildFindPath(Transform child)
        {
            string findPath = child.name;
            
            while (child.parent != null && child.parent.parent != null)
            {
                child = child.parent;
                findPath = child.name + "/" + findPath;
            }

            return findPath;
        }
    }
}