using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UniWork.UIFramework.Runtime.Scheduler;

namespace UniWork.UIFramework.Runtime
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public abstract class UIBaseView : MonoBehaviour
    {
        [ValueDropdown("GetLayerNames")]
        public string layerName;
        public UIScheduleMode scheduleMode = UIScheduleMode.Stack;
        
        private Canvas _canvas;
        private GraphicRaycaster _raycaster;
        private RectTransform _content;

        public Canvas UICanvas => _canvas;
        public GraphicRaycaster UIRaycaster => _raycaster;
        public RectTransform UIContent => _content;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();
            _content = transform.Find("Content").GetComponent<RectTransform>();
        }
        
#if UNITY_EDITOR
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