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
            new AutoBindData("txt", nameof(Text)),
            new AutoBindData("txt", nameof(TextMeshProUGUI)),
            new AutoBindData("btn", nameof(Button)),
            new AutoBindData("img", nameof(Image)),
            new AutoBindData("rawImg", nameof(RawImage)),
            new AutoBindData("trans", nameof(RectTransform)),
            new AutoBindData("go", nameof(GameObject)),
            new AutoBindData("sv", nameof(ScrollRect)),
            new AutoBindData("input", nameof(InputField)),
            new AutoBindData("cg", nameof(CanvasGroup)),
            new AutoBindData("dp", nameof(Dropdown))
        };
    }
}