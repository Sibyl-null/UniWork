#if ODIN_INSPECTOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using System.Text;
using UniWork.UIFramework.Runtime.Scheduler;
using UniWork.Utility.Runtime;

namespace UniWork.UIFramework.Runtime
{
    public partial class UIManager
    {
        [Button, PropertySpace]
        private void PrintAllInstantiatedUI()
        {
            IEnumerable<string> instantiatedUIs = _instantiatedCtrls.Keys.Select(type => type.value);
            
            int index = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("AllInstantiatedUI: ");
            
            foreach (string uiName in instantiatedUIs)
            {
                sb.AppendLine($"{index++}: {uiName}");
            }
            
            DLog.Info(sb.ToString());
        }
        
        [Button, PropertySpace]
        private void PrintAllInStackUI()
        {
            var uiStack = (_schedulers[UIScheduleMode.Stack] as UIStackScheduler).UiStack;
            
            int index = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("AllInStackUI: ");
            
            foreach (UIBaseType type in uiStack)
            {
                sb.AppendLine($"{index++}: {type.value}");
            }
            
            DLog.Info(sb.ToString());
        }

        [Button, PropertySpace]
        private void PrintAllInQueueUI()
        {
            var queueUI = (_schedulers[UIScheduleMode.Queue] as UIQueueScheduler).UiQueue;
            
            int index = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("AllInQueueUI: ");
            
            foreach (UIBaseType type in queueUI)
            {
                sb.AppendLine($"{index++}: {type.value}");
            }
            
            DLog.Info(sb.ToString());
        }
    }
}
#endif