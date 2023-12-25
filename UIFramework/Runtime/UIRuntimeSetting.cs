using System.Collections.Generic;
using System.Linq;
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

        public GameObject rootPrefab;
        
        [TitleGroup("UI 层级配置")]
        public int layerOrderOnceRaise = 5;

        [TitleGroup("UI 层级配置")]
        [ValueDropdown(nameof(GetAllSortingLayers))]
        public string sortingLayerName = "Default";
        
        [TitleGroup("UI 层级配置")]
        [InfoBox("间距尽量大，范围 -32768 ~ 32767")]
        [TableList(ShowIndexLabels = true)]
        public List<ShowLayer> showLayers = new List<ShowLayer>();
        
        private string[] GetAllSortingLayers()
        {
            return SortingLayer.layers.Select(x => x.name).ToArray();
        }
    }
}