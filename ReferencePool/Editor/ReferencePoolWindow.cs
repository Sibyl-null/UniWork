using System;
using System.Collections.Generic;
using System.Reflection;
using SFramework.ReferencePool.Runtime;
using UnityEditor;
using UnityEngine;

namespace SFramework.ReferencePool.Editor
{
    public class ReferencePoolWindow : EditorWindow
    {
        [MenuItem("SFramework/ReferencePool/打开ReferencePoolWindow面板")]
        public static void Open()
        {
            GetWindow<ReferencePoolWindow>().Show();
        }

        private FieldInfo _collectorsField;

        private void OnEnable()
        { 
            _collectorsField = typeof(Runtime.ReferencePool).GetField("_collectors", 
                BindingFlags.NonPublic | BindingFlags.Static);

            if (_collectorsField == null)
                throw new Exception("_collectors字段反射获取失败");
        }

        private void OnGUI()
        {
            Dictionary<Type, ReferenceCollector> collectors =
                (Dictionary<Type, ReferenceCollector>)_collectorsField.GetValue(null);
            
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("缓存类型 Type", GUILayout.Width(200));
                GUILayout.Label("外部数量", GUILayout.Width(100));
                GUILayout.Label("池内数量", GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();

            foreach (ReferenceCollector collector in collectors.Values)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    GUILayout.Label(collector.ReferenceType.FullName, GUILayout.Width(200));
                    GUILayout.Label(collector.OutSideCount.ToString(), GUILayout.Width(100));
                    GUILayout.Label(collector.InSideCount.ToString(), GUILayout.Width(100));
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}