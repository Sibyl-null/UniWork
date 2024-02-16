using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UniWork.UIFramework.Runtime;

namespace UniWork.UIFramework.Editor.Drawers
{
    public static class UIComponentHierarchy
    {
        [InitializeOnLoadMethod]
        private static void StartInitializeOnLoadMethod()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
                return;

            UIComponentCollector collector = go.GetComponentInParent<UIComponentCollector>();
            if (collector == null || collector.gameObject == go)
                return;

            List<Object> components = go.GetComponents<Component>().Cast<Object>().ToList();
            components.Add(go);

            if (collector.ComponentInfos.Count(x => components.Contains(x.component)) > 0)
            {
                Rect r = new Rect(selectionRect)
                {
                    x = 34,
                    width = 80
                };
                
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.yellow;
                
                GUI.Label(r, "★", style);
            }
        }
    }
}