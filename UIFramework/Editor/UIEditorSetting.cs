using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFramework.UIFramework.Editor
{
    public class UIEditorSetting : ScriptableObject
    {
        [Serializable]
        public struct AutoBindData
        {
            public string prefix;
            public string componentName;
        }

        [FolderPath]
        public string codeFileSavePath = "Scripts/AutoGen/UI";
        public string codeNamespace = "UI";
        public List<AutoBindData> autoBindComponents = new List<AutoBindData>();
    }
}