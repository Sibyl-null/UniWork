using System;
using UniWork.Utility.Runtime;

namespace UniWork.UIFramework.Runtime
{
    public abstract class UIBaseType : EnumBaseType<UIBaseType>
    {
        protected UIBaseType(int key, string value) : base(key, value)
        {
        }
    }

    public enum UIScheduleMode
    {
        Normal, Queue, Stack
    }

    public class UIInfo
    {
        public UIBaseType UIBaseType;
        public string LayerName;
        public UIScheduleMode ScheduleMode;
        public string ResPath;

        public UIInfo(UIBaseType uiBaseType, string layerName, 
            Type ctrlType, UIScheduleMode scheduleMode, string resPath)
        {
            UIBaseType = uiBaseType;
            LayerName = layerName;
            ScheduleMode = scheduleMode;
            ResPath = resPath;
        }
    }
}