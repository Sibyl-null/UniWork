using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UniWork.UniBundle.Editor.BuildContexts;
using UniWork.UniBundle.Editor.SettingDefine;

namespace UniWork.UniBundle.Editor.BuildTasks
{
    internal class CollectAssetsTask : IBuildTask
    {
        private static readonly List<string> IgnoreExtensions = new List<string>()
        {
            ".meta", ".DS_Store"
        };

        private AssetInfoContainer _container;
        
        public void Run()
        {
            _container = new AssetInfoContainer();
            BuildContext.SetContextData(_container);

            string[] files = Directory.GetFiles(Path.Combine("Assets", BundleEditorDefine.BundleFolder), 
                "*", SearchOption.AllDirectories).Where(p =>
            {
                string extension = Path.GetExtension(p);
                return !IgnoreExtensions.Contains(extension);
            }).ToArray();

            foreach (string file in files)
                ProcessFile(file);
        }

        private void ProcessFile(string file)
        {
            string[] deps = AssetDatabase.GetDependencies(file);
            foreach (string dep in deps)
                _container.RecordAsset(dep);
        }
    }
}