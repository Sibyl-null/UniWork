using System.Collections.Generic;
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
        public List<ShowLayer> showLayers = new List<ShowLayer>();
    }
}