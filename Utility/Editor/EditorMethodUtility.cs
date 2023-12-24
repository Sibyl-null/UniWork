using System.IO;
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
        public static void CreateScriptableObjectAsset<T>(string savePath) where T : ScriptableObject
        {
            if (File.Exists(savePath))
            {
                DLog.Info("[Utility] 目标路径已存在文件 " + savePath);
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
    }
}