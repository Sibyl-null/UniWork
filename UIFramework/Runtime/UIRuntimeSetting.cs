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

        public int layerOrderOnceRaise = 10;
        public GameObject rootPrefab;
        
        [Title("UI 层级配置 (间距尽量大，填满 0 ~ 23e)"), TableList(ShowIndexLabels = true)]
        public List<ShowLayer> showLayers = new List<ShowLayer>();
    }
}