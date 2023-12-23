namespace UniWork.UIFramework.Runtime
{
    public enum UIScheduleMode
    {
        Normal, Queue, Stack
    }

    public class UIInfo
    {
        public string LayerName;
        public UIScheduleMode ScheduleMode;
        public string ResPath;

        public UIInfo(string layerName, UIScheduleMode scheduleMode, string resPath)
        {
            LayerName = layerName;
            ScheduleMode = scheduleMode;
            ResPath = resPath;
        }
    }
}