using System;
using Cysharp.Threading.Tasks;

namespace SFramework.BehaviourTree.Runtime.Node.Action
{
    public class DelayNode : BaseActionNode
    {
        public float delayTime;
        
        protected override void OnStart()
        {
            DelayTime(delayTime).Forget();
        }

        protected override void OnCancel()
        {
        }

        private async UniTaskVoid DelayTime(float time)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time));
            Finish(true);
        }
    }
}