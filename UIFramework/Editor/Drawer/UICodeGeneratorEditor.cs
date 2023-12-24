using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UniWork.UIFramework.Editor.CodeGenerators;
using UniWork.UIFramework.Runtime;
using UniWork.Utility.Runtime;

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

            if (GUILayout.Button("挂载并绑定 UIView"))
            {
                AddViewScriptAndBindComponent();
            }

            if (GUILayout.Button("初始化创建: View + Ctrl + Config"))
            {
                UIViewGenerator.GenerateCode(Target.gameObject);
                UICtrlGenerator.GenerateCode(Target.gameObject);
                UIConfigGenerator.GenerateCode();
            }
        }
        
        private void AddViewScriptAndBindComponent()
        {
            UIEditorSetting editorSetting = UIEditorSetting.MustLoad();
            string scriptPath = $"{editorSetting.codeFileRootPath}/{Target.name}/{Target.name}View.cs";
            
            if (File.Exists(scriptPath) == false)
            {
                DLog.Error("[UIFramework] View 脚本不存在，挂载失败: " + scriptPath);
                return;
            }
            
            Type scriptType = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath).GetClass();
            Component component = Target.gameObject.GetOrAddComponent(scriptType);
            
            MethodInfo methodInfo =
                scriptType.GetMethod("BindComponent", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(component, new object[] { });

            DLog.Info($"[UIFramework] View 脚本添加绑定成功: {scriptType.Name}.cs");
        }
    }
}