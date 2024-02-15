using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UniWork.UIFramework.Runtime
{
    public class UICodeGenerator : MonoBehaviour
    {
        [Serializable]
        private class ComponentInfo
        {
            public UnityEngine.Object Component;
            public string FieldName;
        }

        [TableList(AlwaysExpanded = true), SerializeField]
        private List<ComponentInfo> components = new List<ComponentInfo>();
        
#if UNITY_EDITOR
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
                    ComponentInfo info = components.Find(x => x.Component == component);
                    info.FieldName = fieldName;
                    return;
                }
            
                components.Add(new ComponentInfo
                {
                    Component = component,
                    FieldName = fieldName
                });
            }
        }

        private void RemoveComponentInfo(UnityEngine.Object component)
        {
            if (ContainComponent(component) == false)
                return;
            
            ComponentInfo info = components.Find(x => x.Component == component);
            components.Remove(info);
        }

        public bool ContainComponent(UnityEngine.Object component)
        {
            return components.Count(x => x.Component == component) > 0;
        }
#endif
    }
}