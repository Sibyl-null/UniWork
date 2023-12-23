namespace UniWork.UIFramework.Runtime
{
    public enum UIScheduleMode
    {
        Normal, Queue, Stack
    }

    public class UIInfo
    {
        public string LayerName { get; }
        public UIScheduleMode ScheduleMode { get; }
        public string ResPath { get; }

        public UIInfo(string layerName, UIScheduleMode scheduleMode, string resPath)
        {
            LayerName = layerName;
            ScheduleMode = scheduleMode;
            ResPath = resPath;
        }
    }
}