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
    internal static class UIConfigGenerator
    {
        private struct InfoData
        {
            public string CtrlTypeName;
            public string LayerName;
            public string ResPath;

            public InfoData(string ctrlTypeName, string layerName, string resPath)
            {
                CtrlTypeName = ctrlTypeName;
                LayerName = layerName;
                ResPath = resPath;
            }
        }

        private struct ConfigGenerateData
        {
            public string[] Namespaces;
            public string YourNamespace;
            public InfoData[] Infos;
        }

        private const string TemplatePath = "ScribanTemplates/UIConfigTemplate";
        
        public static void GenerateCode()
        {
            ConfigGenerateData data = CollectGenerateData();
            GenerateAndSaveCode(data);
        }

        private static ConfigGenerateData CollectGenerateData()
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();

            List<InfoData> infoList = new List<InfoData>();
            HashSet<string> namespaceSet = new HashSet<string>();

            foreach ((UIComponentCollector view, string path) in GetAllViews())
            {
                string resPath = editorSetting.resPathWithExtension ? path : IoUtility.FilePathRemoveExtension(path);
                resPath = resPath.RemovePrefix(editorSetting.resPathRemovePrefix);
                
                namespaceSet.Add($"{editorSetting.rootNamespace}.{view.name}");
                infoList.Add(new InfoData($"{view.name}Ctrl", view.LayerName, resPath));
            }

            ConfigGenerateData data = new ConfigGenerateData
            {
                Namespaces = namespaceSet.ToArray(),
                YourNamespace = editorSetting.rootNamespace,
                Infos = infoList.ToArray()
            };
            return data;
        }
        
        private static void GenerateAndSaveCode(ConfigGenerateData data)
        {
            string code = EditorMethodUtility.ScribanGenerateText(TemplatePath, data);

            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            string filePath = $"{editorSetting.codeFileRootPath}/UIConfig.cs";
            
            IoUtility.OverlayWriteTextFile(filePath, code);
            
            AssetDatabase.Refresh();
            DLog.Info("[自动生成 UIConfig 代码]: 成功! " + filePath);
        }

        private static List<(UIComponentCollector, string)> GetAllViews()
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            List<(UIComponentCollector, string)> results = new List<(UIComponentCollector, string)>();
            
            string[] guids = AssetDatabase.FindAssets("t:Prefab",
                editorSetting.prefabSearchFolders.Select(AssetDatabase.GetAssetPath).ToArray());
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab.TryGetComponent<UIComponentCollector>(out var view))
                {
                    results.Add((view, path));
                }
            }

            return results;
        }
    }
}