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

    public abstract class UIBaseLayer : EnumBaseType<UIBaseLayer>
    {
        protected UIBaseLayer(int key, string value) : base(key, value)
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
        public Type CtrlType;
        public UIScheduleMode ScheduleMode;
        public string ResPath;

        public UIInfo(UIBaseType uiBaseType, string layerName, 
            Type ctrlType, UIScheduleMode scheduleMode, string resPath)
        {
            UIBaseType = uiBaseType;
            LayerName = layerName;
            CtrlType = ctrlType;
            ScheduleMode = scheduleMode;
            ResPath = resPath;
        }
    }
}