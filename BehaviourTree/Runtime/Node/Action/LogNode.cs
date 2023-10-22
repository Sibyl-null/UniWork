using SFramework.BehaviourTree.Runtime.Attribute;
using SFramework.Utility.Runtime;

namespace SFramework.BehaviourTree.Runtime.Node.Action
{
    [NodeInfo(name = "日志节点")]
    public class LogNode : BaseActionNode
    {
        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        public LogLevel logLevel = LogLevel.Info;
        public string logStr;
        
        protected override void OnStart()
        {
            switch (logLevel)
            {
                case LogLevel.Info:
                    DLog.Info(logStr);
                    break;
                case LogLevel.Warning:
                    DLog.Warning(logStr);
                    break;
                case LogLevel.Error:
                    DLog.Error(logStr);
                    break;
            }
            
            Finish(true);
        }

        protected override void OnCancel()
        {
        }
    }
}