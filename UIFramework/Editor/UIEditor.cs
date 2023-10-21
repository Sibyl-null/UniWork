using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SFramework.UIFramework.Runtime;
using SFramework.Utility.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SFramework.UIFramework.Editor
{
    public class UIEditor
    {
        // ---------------------------------------------------------------
        // 初始化资源
        // ---------------------------------------------------------------
        
        public const string UIRootDefaultSavePath = "Assets/Resources/UIRoot.prefab";
        public const string UIRuntimeSettingDefaultSavePath = "Assets/Resources/UIRuntimeSetting.asset";
        public const string UIEditorSettingDefaultSavePath = "Assets/Editor/Config/UIEditorSetting.asset";
        public const string AutoBindTag = "AutoField";

        [MenuItem("SFramework/UIFramework/初始化/创建全部", false, 1)]
        public static void CreateAll()
        {
            CreateUIRootPrefab();
            CreateUIRuntimeSetting();
            CreateUIEditorSetting();
            AddAutoFieldTag();
        }
        
        [MenuItem("SFramework/UIFramework/初始化/创建UIRoot预设体")]
        public static void CreateUIRootPrefab()
        {
            if (File.Exists(UIRootDefaultSavePath))
            {
                DLog.Warning("UIRoot预设体已存在：" + UIRootDefaultSavePath);
                return;
            }

            string foldPath = UIRootDefaultSavePath.Substring(0, UIRootDefaultSavePath.LastIndexOf('/'));
            if (!Directory.Exists(foldPath))
                Directory.CreateDirectory(foldPath);

            GameObject uiRootObj = CreateUIRootObj();
            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(uiRootObj, UIRootDefaultSavePath);
            GameObject.DestroyImmediate(uiRootObj);
            
            AssetDatabase.Refresh();
            Selection.activeObject = prefabAsset;
            EditorGUIUtility.PingObject(prefabAsset);
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

        [MenuItem("SFramework/UIFramework/初始化/创建UIRuntimeSetting")]
        public static void CreateUIRuntimeSetting()
        {
            if (File.Exists(UIRuntimeSettingDefaultSavePath))
            {
                DLog.Warning("UIRuntimeSetting已存在：" + UIRuntimeSettingDefaultSavePath);
                return;
            }

            string foldPath =
                UIRuntimeSettingDefaultSavePath.Substring(0, UIRuntimeSettingDefaultSavePath.LastIndexOf('/'));
            if (!Directory.Exists(foldPath))
                Directory.CreateDirectory(foldPath);
            
            UIRuntimeSetting runtimeSetting = ScriptableObject.CreateInstance<UIRuntimeSetting>();
            AssetDatabase.CreateAsset(runtimeSetting, UIRuntimeSettingDefaultSavePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = runtimeSetting;
            EditorGUIUtility.PingObject(runtimeSetting);
        }

        [MenuItem("SFramework/UIFramework/初始化/创建UIEditorSetting")]
        public static void CreateUIEditorSetting()
        {
            if (File.Exists(UIEditorSettingDefaultSavePath))
            {
                DLog.Warning("UIEditorSetting已存在：" + UIEditorSettingDefaultSavePath);
                return;
            }

            string foldPath =
                UIEditorSettingDefaultSavePath.Substring(0, UIEditorSettingDefaultSavePath.LastIndexOf('/'));
            if (!Directory.Exists(foldPath))
                Directory.CreateDirectory(foldPath);
            
            UIEditorSetting editorSetting = ScriptableObject.CreateInstance<UIEditorSetting>();
            AssetDatabase.CreateAsset(editorSetting, UIEditorSettingDefaultSavePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = editorSetting;
            EditorGUIUtility.PingObject(editorSetting);
        }

        [MenuItem("SFramework/UIFramework/初始化/添加AutoField Tag")]
        public static void AddAutoFieldTag()
        {
            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset"));
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; ++i)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == AutoBindTag)
                {
                    DLog.Warning("已存在该Tag: " + AutoBindTag);
                    return;
                }
            }

            int index = tagsProp.arraySize;
            tagsProp.InsertArrayElementAtIndex(index);
            SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
            sp.stringValue = AutoBindTag;

            tagManager.ApplyModifiedProperties();
            DLog.Info("成功添加Tag: " + AutoBindTag);
        }
        
        
        // ---------------------------------------------------------------
        // 工作流
        // ---------------------------------------------------------------

        private const int MenuItemPriority = 100;
        private const string AutoGenScriptNameKey = "AutoGenScriptName";
        
        [MenuItem("Assets/创建UITemplate模板", false, MenuItemPriority)]
        public static void CreateUITemplate()
        {
            string savePath = GetSelectedPath();
            if (savePath == "" || Directory.Exists(savePath) == false)
                throw new Exception("[CreateUITemplate]: 请选择一个文件夹");

            // create gameObject
            GameObject uiTemplate = new GameObject("ZTemplateUI");
            uiTemplate.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = uiTemplate.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            
            uiTemplate.AddComponent<GraphicRaycaster>();

            GameObject contentObj = new GameObject("Content");
            RectTransform contentTrans = contentObj.AddComponent<RectTransform>();
            contentTrans.SetParent(uiTemplate.transform);
            contentTrans.offsetMin = Vector2.zero;
            contentTrans.offsetMax = Vector2.zero;
            contentTrans.anchorMin = Vector2.zero;
            contentTrans.anchorMax = Vector2.one;

            // save as prefab
            GameObject prefabAsset =
                PrefabUtility.SaveAsPrefabAsset(uiTemplate, Path.Combine(savePath, "ZTemplateUI.prefab"));
            GameObject.DestroyImmediate(uiTemplate);
            
            AssetDatabase.Refresh();
            Selection.activeObject = prefabAsset;
            EditorGUIUtility.PingObject(prefabAsset);
        }

        private static string GetSelectedPath()
        {
            string selectedPath = "";
            
            if (Selection.assetGUIDs.Length > 0)
                selectedPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            
            return selectedPath;
        }
        
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

            // 2. 数据收集
            HashSet<string> namespaceSet = new HashSet<string> { "SFramework.UIFramework.Runtime" };
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
            
            // 3. 代码生成
            StringBuilder sb = new StringBuilder();
            
            // generate code info
            sb.AppendLine($"// Auto generate at {DateTime.Now.Date}");
            sb.AppendLine("// please do not modify this file");
            sb.AppendLine();
            
            // namespace
            foreach (string space in namespaceSet)
                sb.AppendLine($"using {space};");
            sb.AppendLine();

            if (string.IsNullOrEmpty(editorSetting.codeNamespace) == false)
            {
                sb.AppendLine($"namespace {editorSetting.codeNamespace}");
                sb.AppendLine("{");
            }

            // class name
            sb.AppendLine($"\tpublic partial class {selectedObject.name}View : UIBaseView");
            sb.AppendLine("\t{");
            
            // field
            foreach (var tuple in fieldList)
                sb.AppendLine($"\t\tpublic {tuple.typeName} {tuple.fieldName};");

            // bind method
            sb.AppendLine();
            sb.AppendLine("\t\t// only editor use");
            sb.AppendLine("\t\tprivate void BindComponent()");
            sb.AppendLine("\t\t{");
            
            foreach (var p in goNamePathMap)
                sb.AppendLine($"\t\t\tvar {p.Key} = transform.Find(\"{p.Value}\");");
            sb.AppendLine();
            foreach (var tuple in fieldList)
            {
                sb.AppendLine(tuple.typeName != nameof(GameObject)
                    ? $"\t\t\t{tuple.fieldName} = {tuple.goName}.GetComponent<{tuple.typeName}>();"
                    : $"\t\t\t{tuple.fieldName} = {tuple.goName}.gameObject;");
            }

            sb.AppendLine("\t\t}");

            // end
            sb.AppendLine("\t}");
            if (string.IsNullOrEmpty(editorSetting.codeNamespace) == false)
                sb.AppendLine("}");

            string savePath = Path.Combine(editorSetting.codeFileSavePath, $"{selectedObject.name}View.cs");
            if (Directory.Exists(editorSetting.codeFileSavePath) == false)
                Directory.CreateDirectory(editorSetting.codeFileSavePath);
            
            File.WriteAllText(savePath, sb.ToString());
            
            AssetDatabase.Refresh();
            DLog.Info("[自动生成UIView代码]: 成功! " + savePath);
            
            // 4. 脚本挂载并绑定
            EditorPrefs.SetString(AutoGenScriptNameKey, $"{selectedObject.name}View");
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