using SFramework.Utility.Runtime;

namespace SFramework.BehaviourTree.Runtime.Node.Action
{
    public class LogNode : BaseActionNode
    {
        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        private LogLevel _level = LogLevel.Info;
        private string _log;
        
        protected override void OnStart()
        {
            switch (_level)
            {
                case LogLevel.Info:
                    DLog.Info(_log);
                    break;
                case LogLevel.Warning:
                    DLog.Warning(_log);
                    break;
                case LogLevel.Error:
                    DLog.Error(_log);
                    break;
            }
            
            Finish(true);
        }

        protected override void OnCancel()
        {
        }
    }
}