using System;
using System.IO;
using Scriban;
using Scriban.Runtime;
using UnityEditor;
using UnityEngine;
using UniWork.Utility.Runtime;

namespace UniWork.Utility.Editor
{
    /// <summary>
    /// Editor 下的各种帮助方法
    /// </summary>
    public static class EditorMethodUtility
    {
        /// <summary>
        /// 创建并保存 SO 文件
        /// </summary>
        public static void CreateScriptableObjectAsset<T>(string savePath) where T : ScriptableObject
        {
            if (File.Exists(savePath))
            {
                DLog.Warning("[Utility] 目标路径已存在文件 " + savePath);
                return;
            }

            string directoryName = Path.GetDirectoryName(savePath);
            if (Directory.Exists(directoryName) == false)
                Directory.CreateDirectory(directoryName);

            T so = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(so, savePath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(so);
            DLog.Info($"[Utility] {savePath} 创建成功");
        }
        
        /// <summary>
        /// 获取当前选择资源的路径
        /// </summary>
        public static string GetSelectedPath()
        {
            string selectedPath = "";
            
            if (Selection.assetGUIDs.Length > 0)
                selectedPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            
            return selectedPath;
        }

        /// <summary>
        /// 使用 Scriban 模板生成文本
        /// </summary>
        /// <param name="templatePath">目标路径，必须在 Resources 下</param>
        /// <param name="data">传入的外部数据</param>
        public static string ScribanGenerateText(string templatePath, object data)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(templatePath);

            ScriptObject scriptObject = new ScriptObject();
            scriptObject.Import(data);

            TemplateContext context = new TemplateContext();
            context.PushGlobal(scriptObject);

            Template template = Template.Parse(textAsset.text);
            if (template.HasErrors)
            {
                foreach (var error in template.Messages)
                    DLog.Error(error.ToString());

                throw new Exception("文本生成失败，Scriban 模板解析出错");
            }
            
            return template.Render(context);
        }
    }
}