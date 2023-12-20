using UnityEditor;
using UniWork.UniBundle.Editor.BundleCollection.BuildContexts;
using UniWork.UniBundle.Editor.BundleCollection.BuildTasks;
using UniWork.UniBundle.Editor.SettingDefine;

namespace UniWork.UniBundle.Editor.BundleCollection
{
    public static class BundleBuilder
    {
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