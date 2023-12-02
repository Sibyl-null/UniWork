using UnityEditor;
using UniWork.UniBundle.Editor.BundleCollection;
using UniWork.UniBundle.Editor.SettingDefine;
using UniWork.Utility.Editor;

namespace UniWork.UniBundle.Editor
{
    internal static class BundleMenuItem
    {
        [MenuItem("UniWork/UniBundle/打包 AB")]
        private static void BundleAssetBundles()
        {
            BundleBuilder.BuildAssetBundle();
        }
        
        [MenuItem("UniWork/UniBundle/创建 EditorSetting")]
        private static void CreateEditorSetting()
        {
            EditorMethodUtility.CreateScriptableObjectAsset<BundleEditorSetting>(BundleEditorSetting.LoadPath);
        }
    }
}