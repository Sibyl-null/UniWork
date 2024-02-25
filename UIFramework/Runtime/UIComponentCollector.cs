using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UniWork.UIFramework.Runtime
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public sealed class UIComponentCollector : MonoBehaviour
    {
        [Serializable]
        public class ComponentInfo
        {
            public UnityEngine.Object component;
            public string propertyName;
        }

        [ValueDropdown("GetLayerNames"), SerializeField]
        private string _layerName;
        
        [TableList(AlwaysExpanded = true), SerializeField]
        private List<ComponentInfo> componentInfos = new List<ComponentInfo>();

        public string LayerName => _layerName;
        public List<ComponentInfo> ComponentInfos => componentInfos;
        
        
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
                    ComponentInfo info = componentInfos.Find(x => x.component == component);
                    info.propertyName = fieldName;
                    return;
                }
            
                componentInfos.Add(new ComponentInfo
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
            
            ComponentInfo info = componentInfos.Find(x => x.component == component);
            componentInfos.Remove(info);
        }

        public bool ContainComponent(UnityEngine.Object component)
        {
            return componentInfos.Count(x => x.component == component) > 0;
        }
#endif
    }
}