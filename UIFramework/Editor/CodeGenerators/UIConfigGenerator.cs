using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Scriban;
using Scriban.Runtime;
using UnityEditor;
using UnityEngine;
using UniWork.UIFramework.Runtime;
using UniWork.Utility.Runtime;

namespace UniWork.UIFramework.Editor.CodeGenerators
{
    internal static class UIConfigGenerator
    {
        private struct InfoData
        {
            public string CtrlTypeName;
            public string LayerName;
            public string ScheduleMode;
            public string ResPath;

            public InfoData(string ctrlTypeName, string layerName, string scheduleMode, string resPath)
            {
                CtrlTypeName = ctrlTypeName;
                LayerName = layerName;
                ScheduleMode = scheduleMode;
                ResPath = resPath;
            }
        }

        private struct ConfigGenerateData
        {
            public string NowDateTime;
            public string[] Namespaces;
            public string YourNamespace;
            public InfoData[] Infos;
        }
        
        [MenuItem("UniWork/UIFramework/自动生成 UIConfig 代码")]
        private static void GenerateConfigCode()
        {
            ConfigGenerateData data = CollectGenerateData();
            ScribanGenerateCode(data);
        }

        private static ConfigGenerateData CollectGenerateData()
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();

            List<InfoData> infoList = new List<InfoData>();
            HashSet<string> namespaceSet = new HashSet<string>();
            
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab.TryGetComponent<UICodeGenerator>(out var generator))
                {
                    namespaceSet.Add($"{editorSetting.rootNamespace}.{generator.name}");
                    infoList.Add(new InfoData($"{generator.name}Ctrl", generator.layerName,
                        generator.scheduleMode.ToString(), path));
                }
            }

            ConfigGenerateData data = new ConfigGenerateData
            {
                NowDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Namespaces = namespaceSet.ToArray(),
                YourNamespace = editorSetting.rootNamespace,
                Infos = infoList.ToArray()
            };
            return data;
        }
        
        private static void ScribanGenerateCode(ConfigGenerateData data)
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            TextAsset textAsset = Resources.Load<TextAsset>("UIConfigTemplate");

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

            string savePath = Path.Combine(editorSetting.codeFileSavePath, "UIConfig.cs");
            if (Directory.Exists(editorSetting.codeFileSavePath) == false)
                Directory.CreateDirectory(editorSetting.codeFileSavePath);
            
            File.WriteAllText(savePath, code);
            
            AssetDatabase.Refresh();
            DLog.Info("[自动生成 UIConfig 代码]: 成功! " + savePath);
        }
    }
}