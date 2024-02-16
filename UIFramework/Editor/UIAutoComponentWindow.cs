using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UniWork.UIFramework.Runtime;

namespace UniWork.UIFramework.Editor
{
    public class UIAutoComponentWindow : OdinEditorWindow
    {
        [MenuItem("GameObject/Open UIAutoComponentWindow", false, -1000)]
        private static void HierarchyItem()
        {
            GetWindow<UIAutoComponentWindow>("UIAutoComponentWindow");
        }
        
        [MenuItem("GameObject/Open UIAutoComponentWindow", true, -1000)]
        private static bool ValidHierarchyItem()
        {
            // 必须处于编辑 Prefab 阶段
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                return false;

            // 必须选中 Hierarchy 中的对象
            Transform trans = Selection.activeTransform;
            if (trans == null)
                return false;

            // 根节点必须拥有 UIBaseView 组件，且自己不是根节点
            UIComponentCollector componentCollector = trans.GetComponentInParent<UIComponentCollector>(true);
            if (componentCollector == null || componentCollector.transform == trans)
                return false;

            return true;
        }

        [System.Serializable]
        private struct ComponentInfo
        {
            public UnityEngine.Object Component;
            public string PropertyName;
            public bool IsSelect;
        }

        private UIEditorSetting _editorSetting;
        private UIComponentCollector _componentCollector;
        
        [TableList(AlwaysExpanded = true, HideToolbar = true), SerializeField]
        private List<ComponentInfo> _infos = new List<ComponentInfo>();
        
        protected override void OnEnable()
        {
            base.OnEnable();
            if (ValidHierarchyItem() == false)
                return;
            
            _editorSetting = UIEditorSetting.MustLoad();
            Transform trans = Selection.activeTransform;
            _componentCollector = trans.GetComponentInParent<UIComponentCollector>(true);
            
            _infos.Clear();
            var components = trans.GetComponents<Component>();
            foreach (Component component in components)
            {
                _infos.Add(new ComponentInfo()
                {
                    Component = component,
                    PropertyName = GetFieldName(component),
                    IsSelect = _componentCollector.ContainComponent(component)
                });
            }
            
            _infos.Add(new ComponentInfo()
            {
                Component = trans.gameObject,
                PropertyName = GetFieldName(trans.gameObject),
                IsSelect = _componentCollector.ContainComponent(trans.gameObject)
            });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _infos.Clear();
            _componentCollector = null;
        }

        [Button("确认修改")]
        private void ApplyModify()
        {
            foreach (ComponentInfo info in _infos)
            {
                _componentCollector.ModifyComponentInfo(info.Component, info.PropertyName, info.IsSelect);
            }
            EditorUtility.SetDirty(_componentCollector);
            Debug.Log("UIAutoComponentWindow 修改引用成功");
            
            Close();
        }

        private string GetFieldName(UnityEngine.Object component)
        {
            string typeName = component.GetType().Name;

            if (_editorSetting.autoBindComponents.Count(x => x.componentName == typeName) > 0)
            {
                var data = _editorSetting.autoBindComponents.First(x => x.componentName == typeName);
                return $"{data.prefix}{component.name}";
            }
            
            return $"{typeName}{component.name}";
        }
    }
}