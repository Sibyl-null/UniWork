using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SFramework.UIFramework.Editor
{
    public class UIEditorSetting : ScriptableObject
    {
        [Serializable]
        public struct AutoBindData
        {
            public string prefix;
            public string componentName;

            public AutoBindData(string prefix, string componentName)
            {
                this.prefix = prefix;
                this.componentName = componentName;
            }
        }

        [FolderPath]
        public string codeFileSavePath = "Assets/Scripts/AutoGen/UI";
        public string codeNamespace = "UI";

        public List<AutoBindData> autoBindComponents = new List<AutoBindData>()
        {
            new AutoBindData("txt_", nameof(Text)),
            new AutoBindData("txt_", nameof(TextMeshProUGUI)),
            new AutoBindData("btn_", nameof(Button)),
            new AutoBindData("img_", nameof(Image)),
            new AutoBindData("rawImg_", nameof(RawImage)),
            new AutoBindData("trans_", nameof(RectTransform)),
            new AutoBindData("go_", nameof(GameObject)),
            new AutoBindData("sv_", nameof(ScrollRect)),
            new AutoBindData("input_", nameof(InputField)),
            new AutoBindData("cg_", nameof(CanvasGroup)),
            new AutoBindData("dp_", nameof(Dropdown))
        };
    }
}