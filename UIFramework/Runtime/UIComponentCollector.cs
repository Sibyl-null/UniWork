using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UniWork.UIFramework.Runtime.Scheduler;

namespace UniWork.UIFramework.Runtime
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public sealed class UIComponentCollector : MonoBehaviour
    {
        [Serializable]
        private class ComponentInfo
        {
            public UnityEngine.Object component;
            public string propertyName;
        }

        [ValueDropdown("GetLayerNames"), SerializeField]
        private string _layerName;
        
        [SerializeField] 
        private UIScheduleMode _scheduleMode = UIScheduleMode.Stack;
        
        [TableList(AlwaysExpanded = true), SerializeField]
        private List<ComponentInfo> _components = new List<ComponentInfo>();

        public string LayerName => _layerName;
        public UIScheduleMode ScheduleMode => _scheduleMode;
        public List<UnityEngine.Object> Components => _components.Select(x => x.component).ToList();
        
        
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
        
        public void ModifyComponentInfo(UnityEngine.Object component, string fieldName, bool isSelect)
        {
            if (isSelect == false)
            {
                RemoveComponentInfo(component);
            }
            else
            {
                if (ContainComponent(component))
                {
                    ComponentInfo info = _components.Find(x => x.component == component);
                    info.propertyName = fieldName;
                    return;
                }
            
                _components.Add(new ComponentInfo
                {
                    component = component,
                    propertyName = fieldName
                });
            }
        }

        private void RemoveComponentInfo(UnityEngine.Object component)
        {
            if (ContainComponent(component) == false)
                return;
            
            ComponentInfo info = _components.Find(x => x.component == component);
            _components.Remove(info);
        }

        public bool ContainComponent(UnityEngine.Object component)
        {
            return _components.Count(x => x.component == component) > 0;
        }
#endif
    }
}