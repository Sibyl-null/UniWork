using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UniWork.UIFramework.Runtime;
using UniWork.Utility.Editor;
using UniWork.Utility.Runtime;
using Object = UnityEngine.Object;

namespace UniWork.UIFramework.Editor
{
    public static class UIEditor
    {
        // ---------------------------------------------------------------
        // 初始化资源
        // ---------------------------------------------------------------

        private const string UIRootDefaultSavePath = "Assets/Resources/UIRoot.prefab";
        private const string UIRuntimeSettingDefaultSavePath = "Assets/Resources/UIRuntimeSetting.asset";
        public const string AutoBindTag = "AutoField";

        [MenuItem("UniWork/UIFramework/创建全部", false, 1)]
        public static void CreateAll()
        {
            CreateUIRootPrefab();
            CreateUIEditorSetting();
            CreateUIRuntimeSetting();
            AddAutoFieldTag();
        }
        
        [MenuItem("UniWork/UIFramework/创建UIRoot预设体")]
        public static void CreateUIRootPrefab()
        {
            if (File.Exists(UIRootDefaultSavePath))
            {
                DLog.Warning("UIRoot 预设体已存在：" + UIRootDefaultSavePath);
                return;
            }

            string folderPath = Path.GetDirectoryName(UIRootDefaultSavePath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            GameObject uiRootObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UIRootTemplate"));
            GameObject asset = PrefabUtility.SaveAsPrefabAsset(uiRootObj, UIRootDefaultSavePath);
            Object.DestroyImmediate(uiRootObj);

            AssetDatabase.Refresh();
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
            DLog.Info("UIRoot 预设体创建成功: " + UIRootDefaultSavePath);
        }

        [MenuItem("UniWork/UIFramework/创建UIEditorSetting")]
        public static void CreateUIEditorSetting()
        {
            UIEditorSetting.CreateAsset();
        }

        [MenuItem("UniWork/UIFramework/创建UIRuntimeSetting")]
        public static void CreateUIRuntimeSetting()
        {
            EditorMethodUtility.CreateScriptableObjectAsset<UIRuntimeSetting>(UIRuntimeSettingDefaultSavePath);
        }

        [MenuItem("UniWork/UIFramework/添加AutoField Tag")]
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
        
        [MenuItem("Assets/创建 UITemplate 模板", false, 100)]
        public static void CreateUITemplate()
        {
            string folderPath = EditorMethodUtility.GetSelectedPath();
            if (folderPath == "" || Directory.Exists(folderPath) == false)
                throw new Exception("[CreateUITemplate]: 请选择一个文件夹");
            
            GameObject uiObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UITemplate"));
            GameObject asset = PrefabUtility.SaveAsPrefabAsset(uiObj, folderPath + "/UITemplate.prefab");
            Object.DestroyImmediate(uiObj);
            
            AssetDatabase.Refresh();
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }
    }
}