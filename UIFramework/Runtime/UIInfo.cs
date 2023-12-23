using UniWork.UIFramework.Runtime.Scheduler;

namespace UniWork.UIFramework.Runtime
{
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