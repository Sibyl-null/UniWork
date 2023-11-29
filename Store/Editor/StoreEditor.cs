using System.IO;
using UnityEditor;
using UniWork.Store.Runtime;
using UniWork.Utility.Runtime;

namespace UniWork.Store.Editor
{
    public static class StoreEditor
    {
        [MenuItem("UniWork/Store/清除 Store 数据")]
        public static void ClearStoreData()
        {
            if (File.Exists(StoreRoot.StorePath))
            {
                File.Delete(StoreRoot.StorePath);
                DLog.Info("[Store] 数据已清除");
            }
            else
            {
                DLog.Info("[Store] 数据文件不存在");
            }
        }
    }
}