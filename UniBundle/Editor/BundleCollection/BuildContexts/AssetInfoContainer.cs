using System.Collections.Generic;
using System.IO;

namespace UniWork.UniBundle.Editor.BundleCollection.BuildContexts
{
    internal class AssetInfo
    {
        public string Name;
        public string NameNoExt;
        public string AssetPath;
        public int RefCount;
        public string BundleName;

        internal AssetInfo(string assetPath)
        {
            Name = Path.GetFileName(assetPath);
            NameNoExt = Path.GetFileNameWithoutExtension(assetPath);
            AssetPath = assetPath;
            RefCount = 0;
            BundleName = "";
        }
    }
    
    internal class AssetInfoContainer : IContextData
    {
        // AssetPath -> AssetInfo
        internal readonly Dictionary<string, AssetInfo> Infos = new Dictionary<string, AssetInfo>();

        internal void RecordAsset(string assetPath)
        {
            if (Infos.TryGetValue(assetPath, out AssetInfo info))
            {
                ++info.RefCount;
            }
            else
            {
                info = new AssetInfo(assetPath);
                Infos.Add(assetPath, info);
            }
        }
    }
}