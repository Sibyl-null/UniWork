using System;
using Cysharp.Threading.Tasks;

namespace SFramework.BehaviourTree.Runtime.Node.Action
{
    public class DelayNode : BaseActionNode
    {
        private float _delayTime;
        
        protected override void OnStart()
        {
            DelayTime(_delayTime).Forget();
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