using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Scriban;
using Scriban.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UniWork.UIFramework.Runtime;
using UniWork.Utility.Editor;
using UniWork.Utility.Runtime;

namespace UniWork.UIFramework.Editor.CodeGenerators
{
    public static class UIViewGenerator
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
            public string NowDateTime;
            public string YourNamespace;
            public string ClassName;
            public string[] Namespaces;
            public Dictionary<string, string> GoNamePathMap;
            public List<FieldData> Fields;
        }
        
        private const string AutoGenScriptNameKey = "AutoGenScriptName";
        
        [MenuItem("Assets/自动生成UIView代码", false, UIEditor.MenuItemPriority)]
        public static void GenerateUIViewCode()
        {
            GameObject selectedObject = VerifySetting();
            CodeGenerateData data = CollectGenerateData(selectedObject);
            ScribanGenerateCode(data);

            // 脚本挂载并绑定
            EditorPrefs.SetString(AutoGenScriptNameKey, $"{data.ClassName}");
        }

        private static GameObject VerifySetting()
        {
            string selectedPath = EditorMethodUtility.GetSelectedPath();
            if (selectedPath == "" || File.Exists(selectedPath) == false)
                throw new Exception("[自动生成UIView代码]: 请选择一个Prefab");

            GameObject selectedObject = AssetDatabase.LoadAssetAtPath<GameObject>(selectedPath);
            if (selectedObject == null)
                throw new Exception("[自动生成UIView代码]: 请选择一个Prefab");

            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            if (string.IsNullOrEmpty(editorSetting.codeFileSavePath))
                throw new Exception("[自动生成UIView代码]: 代码保存路径未设定");

            return selectedObject;
        }

        private static CodeGenerateData CollectGenerateData(GameObject selectedObject)
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            
            HashSet<string> namespaceSet = new HashSet<string> { typeof(UIBaseView).Namespace };
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
                    throw new Exception("[自动生成UIView代码]: 存在对象重名 " + childObj.name);
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
                NowDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                YourNamespace = editorSetting.rootNamespace,
                ClassName = selectedObject.name + "View",
                Namespaces = namespaceSet.ToArray(),
                GoNamePathMap = goNamePathMap,
                Fields = fieldList
            };
            return data;
        }

        private static void ScribanGenerateCode(CodeGenerateData data)
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            TextAsset textAsset = Resources.Load<TextAsset>("UIViewTemplate");

            ScriptObject scriptObject = new ScriptObject();
            scriptObject.Import(data);

            TemplateContext context = new TemplateContext();
            context.PushGlobal(scriptObject);

            Template template = Template.Parse(textAsset.text);
            if (template.HasErrors)
            {
                foreach (var error in template.Messages)
                    DLog.Error(error.ToString());

                throw new Exception("UIView 生成失败，Scriban 模版解析出错");
            }
            
            string code = template.Render(context);

            string savePath = Path.Combine(editorSetting.codeFileSavePath, $"{data.ClassName}.cs");
            if (Directory.Exists(editorSetting.codeFileSavePath) == false)
                Directory.CreateDirectory(editorSetting.codeFileSavePath);
            
            File.WriteAllText(savePath, code);
            
            AssetDatabase.Refresh();
            DLog.Info("[自动生成UIView代码]: 成功! " + savePath);
        }

        [DidReloadScripts]
        private static void AutoAddComponent()
        {
            GameObject prefab = Selection.activeObject as GameObject;
            string scriptName = EditorPrefs.GetString(AutoGenScriptNameKey, "");

            if (string.IsNullOrEmpty(scriptName) || prefab == null ||
                prefab.name != scriptName.Substring(0, scriptName.Length - 4))
                return;

            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            string scriptPath = Path.Combine(editorSetting.codeFileSavePath, scriptName + ".cs");

            Type scriptType = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath).GetClass();
            if (prefab.GetComponent(scriptType) == null)
                prefab.AddComponent(scriptType);

            Component component = prefab.GetComponent(scriptType);
            MethodInfo methodInfo =
                scriptType.GetMethod("BindComponent", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(component, new object[] { });

            PrefabUtility.SavePrefabAsset(prefab);
            AssetDatabase.Refresh();

            EditorPrefs.SetString(AutoGenScriptNameKey, "");
            DLog.Info($"脚本添加成功: {scriptName}.cs");
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