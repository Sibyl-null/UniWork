using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UniWork.UIFramework.Runtime.Scheduler;

namespace UniWork.UIFramework.Runtime
{
    public class UICodeGenerator : MonoBehaviour
    {
        [ValueDropdown("GetLayerNames")]
        public string layerName;
        public UIScheduleMode scheduleMode = UIScheduleMode.Stack;
        
#if UNITY_EDITOR
        /// <summary>
        /// use by odin ValueDropdown attribute
        /// </summary>
        private static string[] GetLayerNames()
        {
            string guid = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(UIRuntimeSetting)}").FirstOrDefault();
            if (string.IsNullOrEmpty(guid))
                return Array.Empty<string>();

            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            UIRuntimeSetting setting = UnityEditor.AssetDatabase.LoadAssetAtPath<UIRuntimeSetting>(path);
            return setting.showLayers.Select(x => x.name).ToArray();
        }
#endif
    }
}