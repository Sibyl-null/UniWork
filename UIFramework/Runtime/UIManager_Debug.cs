#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System.Linq;
using System.Text;

namespace SFramework.UIFramework.Runtime
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
            
            foreach (UIEnumBaseType type in uiStack)
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
            
            foreach (UIEnumBaseType type in queueUI)
            {
                sb.AppendLine($"{index++}: {type.value}");
            }
            
            DLog.Info(sb.ToString());
        }
    }
}
#endif