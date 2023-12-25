using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UniWork.UIFramework.Runtime
{
    public class UIRuntimeSetting : ScriptableObject
    {
        [System.Serializable]
        public struct ShowLayer
        {
            public string name;
            public int order;
        }

        public int layerOrderOnceRaise = 5;
        public GameObject rootPrefab;
        
        [Title("UI 层级配置 (间距尽量大，范围 -32768 ~ 32767)"), TableList(ShowIndexLabels = true)]
        public List<ShowLayer> showLayers = new List<ShowLayer>();
    }
}