using UnityEditor;
using UnityEngine;
using UniWork.UIFramework.Editor.CodeGenerators;
using UniWork.UIFramework.Runtime;

namespace UniWork.UIFramework.Editor.Drawer
{
    [CustomEditor(typeof(UICodeGenerator))]
    public class UICodeGeneratorEditor : UnityEditor.Editor
    {
        private UICodeGenerator Target => (UICodeGenerator)target;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("重新生成 UIView 代码"))
            {
                UIViewGenerator.GenerateCode(Target.gameObject);
            }
            
            if (GUILayout.Button("重新生成 UICtrl 代码"))
            {
                UICtrlGenerator.GenerateCode(Target.gameObject);
            }

            if (GUILayout.Button("初始化创建: View + Ctrl + Config"))
            {
                InitUICreate();
            }
        }

        private void InitUICreate()
        {
        }
    }
}