namespace UniWork.UIFramework.Runtime
{
    public struct UIInfo
    {
        public string LayerName { get; }
        public string ResPath { get; }

        public UIInfo(string layerName, string resPath)
        {
            LayerName = layerName;
            ResPath = resPath;
        }
    }
}