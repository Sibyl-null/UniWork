#if ODIN_INSPECTOR

using System;
using System.IO;
using System.Text;
using SFramework.Utility.Runtime;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace SFramework.UIFramework.Editor
{
    internal class UICodeAutoGenWindow : OdinEditorWindow
    {
        [MenuItem("SFramework/UIFramework/打开UICodeAutoGen面板")]
        public static void OpenWindow()
        {
            GetWindow<UICodeAutoGenWindow>().Show();
        }

        public string uiName;

        [BoxGroup("View脚本生成设置")]
        public string viewNamespace;
        [FolderPath, BoxGroup("View脚本生成设置")]
        public string viewCodePath;

        [BoxGroup("Ctrl脚本生成设置")]
        public string ctrlNamespace;
        [FolderPath, BoxGroup("Ctrl脚本生成设置")]
        public string ctrlCodePath;

        [Button("生成代码")]
        public void GenCode()
        {
            if (string.IsNullOrWhiteSpace(uiName))
                throw new Exception("UIName不合法");
            
            WriteViewCode();
            WriteCtrlCode();

            DLog.Info("UIView UICtrl生成成功");
            AssetDatabase.Refresh();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            viewNamespace = EditorPrefs.GetString("ViewNamespace", "");
            viewCodePath = EditorPrefs.GetString("ViewCodePath", "");
            ctrlNamespace = EditorPrefs.GetString("CtrlNamespace", "");
            ctrlCodePath = EditorPrefs.GetString("CtrlCodePath", "");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorPrefs.SetString("ViewNamespace", viewNamespace);
            EditorPrefs.SetString("ViewCodePath", viewCodePath);
            EditorPrefs.SetString("CtrlNamespace", ctrlNamespace);
            EditorPrefs.SetString("CtrlCodePath", ctrlCodePath);
        }

        private void WriteViewCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using SFramework.UIFramework.Runtime;");
            sb.AppendLine();
            sb.AppendLine("namespace " + viewNamespace);
            sb.AppendLine("{");
            sb.AppendLine($"\tpublic class {uiName}View : UIBaseView");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            File.WriteAllText(Path.Combine(viewCodePath, $"{uiName}View.cs"), sb.ToString());
        }

        private void WriteCtrlCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using SFramework.UIFramework.Runtime;");
            sb.AppendLine();
            sb.AppendLine("namespace " + ctrlNamespace);
            sb.AppendLine("{");
            sb.AppendLine($"\tpublic class {uiName}Ctrl : UIBaseCtrl");
            sb.AppendLine("\t{");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            File.WriteAllText(Path.Combine(ctrlCodePath, $"{uiName}Ctrl.cs"), sb.ToString());
        }
    }
}

#endif