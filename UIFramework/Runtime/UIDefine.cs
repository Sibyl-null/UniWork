using System;
using SFramework.Utility;

namespace SFramework.UIFramework.Runtime
{
    public abstract class UIEnumBaseType : EnumBaseType<UIEnumBaseType>
    {
        protected UIEnumBaseType(int key, string value) : base(key, value)
        {
        }
    }

    public abstract class UIEnumBaseLayer : EnumBaseType<UIEnumBaseLayer>
    {
        protected UIEnumBaseLayer(int key, string value) : base(key, value)
        {
        }
    }

    public enum UIScheduleMode
    {
        Normal, Queue, Stack
    }

    public class UIInfo
    {
        public UIEnumBaseType UIEnumBaseType;
        public UIEnumBaseLayer UIEnumBaseLayer;
        public Type ViewType;
        public Type CtrlType;
        public UIScheduleMode ScheduleMode;
        public string ResPath;
    }

    public class UIDefine
    {
        public const string UIRootPath = "UIRoot";
    }
}