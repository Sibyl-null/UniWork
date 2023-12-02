using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UniWork.UniBundle.Editor.BundleCollection;
using UniWork.UniBundle.Editor.SettingDefine;
using UniWork.UniBundle.Editor.ShaderVariantCollection;
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

        [MenuItem("UniWork/UniBundle/收集 ShaderVariant")]
        private static void CollectShaderVariant()
        {
            HashSet<string> materials = new HashSet<string>();
            string[] files = Directory.GetFiles(Path.Combine("Assets", BundleEditorDefine.BundleFolder), 
                "*", SearchOption.AllDirectories).Where(p =>
            {
                string extension = Path.GetExtension(p);
                return extension != ".meta" && extension != ".DS_Store";
            }).ToArray();

            foreach (string file in files)
            {
                string[] deps = AssetDatabase.GetDependencies(file);
                foreach (string dep in deps)
                {
                    if (Path.GetExtension(dep) == ".mat")
                        materials.Add(dep);
                }
            }

            ShaderVariantCollector.Run(materials.ToList(), 
                "Assets/AssetBundle/Shaders/svc.shadervariants");
        }
        
        [MenuItem("UniWork/UniBundle/创建 EditorSetting")]
        private static void CreateEditorSetting()
        {
            EditorMethodUtility.CreateScriptableObjectAsset<BundleEditorSetting>(BundleEditorSetting.LoadPath);
        }
    }
}