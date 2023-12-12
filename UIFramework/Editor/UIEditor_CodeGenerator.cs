using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UniWork.UIFramework.Runtime;
using UniWork.Utility.Runtime;

namespace UniWork.UIFramework.Editor
{
    internal class CodeGenerateData
    {
        public string YourNamespace;
        public string ClassName;
        public string[] Namespaces;
        public Dictionary<string, string> GoNamePathMap;
        public List<(string typeName, string fieldName, string goName)> FieldList;
    }
    
    public static partial class UIEditor
    {
        private const string AutoGenScriptNameKey = "AutoGenScriptName";
        
        [MenuItem("Assets/自动生成UIView代码", false, MenuItemPriority)]
        public static void GenerateUIViewCode()
        {
            // 1. 非法情况判断
            string selectedPath = GetSelectedPath();
            if (selectedPath == "" || File.Exists(selectedPath) == false)
                throw new Exception("[自动生成UIView代码]: 请选择一个Prefab");

            GameObject selectedObject = AssetDatabase.LoadAssetAtPath<GameObject>(selectedPath);
            if (selectedObject == null)
                throw new Exception("[自动生成UIView代码]: 请选择一个Prefab");

            UIEditorSetting editorSetting =
                AssetDatabase.LoadAssetAtPath<UIEditorSetting>(UIEditorSettingDefaultSavePath);
            if (editorSetting == null)
                throw new Exception("[自动生成UIView代码]: UIEditorSetting 加载失败");
            if (string.IsNullOrEmpty(editorSetting.codeFileSavePath))
                throw new Exception("[自动生成UIView代码]: 代码保存路径未设定");
            

            CodeGenerateData data = CollectGenerateData(editorSetting, selectedObject);
            string code = GenerateCode(data);

            string savePath = Path.Combine(editorSetting.codeFileSavePath, $"{selectedObject.name}View.cs");
            if (Directory.Exists(editorSetting.codeFileSavePath) == false)
                Directory.CreateDirectory(editorSetting.codeFileSavePath);
            
            File.WriteAllText(savePath, code);
            
            AssetDatabase.Refresh();
            DLog.Info("[自动生成UIView代码]: 成功! " + savePath);
            
            // 4. 脚本挂载并绑定
            EditorPrefs.SetString(AutoGenScriptNameKey, $"{selectedObject.name}View");
        }

        private static CodeGenerateData CollectGenerateData(UIEditorSetting editorSetting, GameObject selectedObject)
        {
            HashSet<string> namespaceSet = new HashSet<string> { typeof(UIBaseView).Namespace };
            Dictionary<string, string> goNamePathMap = new Dictionary<string, string>();
            List<(string typeName, string fieldName, string goName)> fieldList =
                new List<(string typeName, string fieldName, string goName)>();

            List<GameObject> childList = new List<GameObject>();
            GetAllChildGameObjects(selectedObject.transform, ref childList);

            var gameObjectBindData =
                editorSetting.autoBindComponents.Where(bindData => bindData.componentName == nameof(GameObject))
                    .ToArray();
            bool shouldBindGameObject = gameObjectBindData.Length > 0;

            foreach (GameObject childObj in childList)
            {
                if (childObj.CompareTag(AutoBindTag) == false)
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
                        fieldList.Add((autoBindData.componentName, autoBindData.prefix + childObj.name, childObj.name));
                        break;
                    }
                }

                if (shouldBindGameObject)
                {
                    namespaceSet.Add(typeof(GameObject).Namespace);
                    fieldList.Add((nameof(GameObject), gameObjectBindData[0].prefix + childObj.name, childObj.name));
                }
            }

            CodeGenerateData data = new CodeGenerateData
            {
                YourNamespace = editorSetting.codeNamespace,
                ClassName = selectedObject.name,
                Namespaces = namespaceSet.ToArray(),
                GoNamePathMap = goNamePathMap,
                FieldList = fieldList
            };
            return data;
        }

        private static string GenerateCode(CodeGenerateData data)
        {
            StringBuilder sb = new StringBuilder();
            
            // generate code info
            sb.AppendLine($"// Auto generate at {DateTime.Now}");
            sb.AppendLine("// please do not modify this file");
            sb.AppendLine();
            
            // namespace
            foreach (string space in data.Namespaces)
                sb.AppendLine($"using {space};");
            sb.AppendLine();

            if (string.IsNullOrEmpty(data.YourNamespace) == false)
            {
                sb.AppendLine($"namespace {data.YourNamespace}");
                sb.AppendLine("{");
            }

            // class name
            sb.AppendLine($"\tpublic partial class {data.ClassName}View : UIBaseView");
            sb.AppendLine("\t{");
            
            // field
            foreach (var tuple in data.FieldList)
                sb.AppendLine($"\t\tpublic {tuple.typeName} {tuple.fieldName};");

            // bind method
            sb.AppendLine();
            sb.AppendLine("\t\t// only editor use");
            sb.AppendLine("\t\tprivate void BindComponent()");
            sb.AppendLine("\t\t{");
            
            foreach (var p in data.GoNamePathMap)
                sb.AppendLine($"\t\t\tvar {p.Key} = transform.Find(\"{p.Value}\");");
            sb.AppendLine();
            foreach (var tuple in data.FieldList)
            {
                sb.AppendLine(tuple.typeName != nameof(GameObject)
                    ? $"\t\t\t{tuple.fieldName} = {tuple.goName}.GetComponent<{tuple.typeName}>();"
                    : $"\t\t\t{tuple.fieldName} = {tuple.goName}.gameObject;");
            }

            sb.AppendLine("\t\t}");

            // end
            sb.AppendLine("\t}");
            if (string.IsNullOrEmpty(data.YourNamespace) == false)
                sb.AppendLine("}");

            return sb.ToString();
        }

        [DidReloadScripts]
        private static void AutoAddComponent()
        {
            GameObject prefab = Selection.activeObject as GameObject;
            string scriptName = EditorPrefs.GetString(AutoGenScriptNameKey, "");

            if (string.IsNullOrEmpty(scriptName) || prefab == null ||
                prefab.name != scriptName.Substring(0, scriptName.Length - 4))
                return;
            
            UIEditorSetting editorSetting =
                AssetDatabase.LoadAssetAtPath<UIEditorSetting>(UIEditorSettingDefaultSavePath);
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