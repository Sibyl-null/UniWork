using UnityEditor;
using UniWork.UniBundle.Editor.BuildContexts;
using UniWork.UniBundle.Editor.BuildTasks;

namespace UniWork.UniBundle.Editor
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
        }
    }
}