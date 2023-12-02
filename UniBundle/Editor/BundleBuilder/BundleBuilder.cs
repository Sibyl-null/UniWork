using UnityEditor;
using UniWork.UniBundle.Editor.BundleBuilder.BuildContexts;
using UniWork.UniBundle.Editor.BundleBuilder.BuildTasks;
using UniWork.UniBundle.Editor.SettingDefine;

namespace UniWork.UniBundle.Editor.BundleBuilder
{
    public static class BundleBuilder
    {
        [MenuItem("UniWork/UniBundle/打包 AB")]
        public static void BuildAssetBundle()
        {
            BuildContext.Clear();

            IBuildTask[] tasks = {
                new CollectAssetsTask()
            };

            foreach (IBuildTask task in tasks)
                task.Run();
            
            AssetDatabase.Refresh();
            EditorUtility.RevealInFinder(BundleEditorDefine.OutputFolder);
        }
    }
}