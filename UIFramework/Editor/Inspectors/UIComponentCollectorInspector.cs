using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UniWork.UIFramework.Editor.CodeGenerators;
using UniWork.UIFramework.Runtime;

namespace UniWork.UIFramework.Editor.Inspectors
{
    [CustomEditor(typeof(UIComponentCollector))]
    public class UIComponentCollectorInspector : OdinEditor
    {
        private UIComponentCollector Target => (UIComponentCollector)target;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("生成并挂载 View 代码"))
            {
                UIViewGenerator.GenerateCode(Target);
            }
            
            if (GUILayout.Button("初始化创建: View + Ctrl + Config"))
            {
                UIViewGenerator.GenerateCode(Target);
                UICtrlGenerator.GenerateCode(Target.gameObject);
                UIConfigGenerator.GenerateCode();
            }
        }
    }
}