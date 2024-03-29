﻿using System;
using Cysharp.Threading.Tasks;
using UniWork.BehaviourTree.Runtime.Attribute;

namespace UniWork.BehaviourTree.Runtime.Node.Action
{
    [NodeInfo(name = "延时节点")]
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