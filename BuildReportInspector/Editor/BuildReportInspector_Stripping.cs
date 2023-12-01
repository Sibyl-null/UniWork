using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unity.BuildReportInspector
{
    public partial class BuildReportInspector
    {
        private static readonly Dictionary<string, Texture> IconCache = new Dictionary<string, Texture>();

        private readonly Dictionary<string, bool> _strippingReasonsFoldout = new Dictionary<string, bool>();
        private readonly Dictionary<string, Texture> _strippingIcons = new Dictionary<string, Texture>();
        private readonly Dictionary<string, int> _strippingSizes = new Dictionary<string, int>();

        private void OnStrippingGUI()
        {
            if (Report.strippingInfo == null)
            {
                EditorGUILayout.HelpBox("No stripping info.", MessageType.Info);
                return;
            }

            var so = new SerializedObject(Report.strippingInfo);
            var serializedDependencies = so.FindProperty("serializedDependencies");

            if (serializedDependencies != null)
            {
                for (var i = 0; i < serializedDependencies.arraySize; i++)
                {
                    var sp = serializedDependencies.GetArrayElementAtIndex(i);
                    var depKey = sp.FindPropertyRelative("key").stringValue;
                    _strippingIcons[depKey] = StrippingEntityIcon(sp.FindPropertyRelative("icon").stringValue);
                    _strippingSizes[depKey] = sp.FindPropertyRelative("size").intValue;
                }
            }

            var analyzeMethod = Report.strippingInfo.GetType().GetMethod("Analyze", System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (/*!hasSizes &&*/ analyzeMethod != null)
            {
                if (GUILayout.Button("Analyze size"))
                    analyzeMethod.Invoke(Report.strippingInfo, null);
            }

            var odd = false;
            foreach (var module in Report.strippingInfo.includedModules)
            {
                StrippingEntityGui(module, ref odd);
            }
        }
        
        private void StrippingEntityGui(string entity, ref bool odd)
        {
            GUILayout.BeginHorizontal(odd ? GUIStyleUtilities.OddStyle : GUIStyleUtilities.EvenStyle);
            odd = !odd;
            GUILayout.Space(15); 
            var reasons = Report.strippingInfo.GetReasonsForIncluding(entity).ToList();
            if (!_strippingIcons.ContainsKey(entity))
                _strippingIcons[entity] = StrippingEntityIcon(entity);
            var icon = _strippingIcons[entity];
            if (reasons.Any())
            {
                if (!_strippingReasonsFoldout.ContainsKey(entity))
                    _strippingReasonsFoldout[entity] = false;
                _strippingReasonsFoldout[entity] = EditorGUILayout.Foldout(_strippingReasonsFoldout[entity], new GUIContent(entity, icon));
            }
            else
                EditorGUILayout.LabelField(new GUIContent(entity, icon), GUILayout.Height(16), GUILayout.MaxWidth(1000));

            GUILayout.FlexibleSpace();

            if (_strippingSizes.ContainsKey(entity) && _strippingSizes[entity] != 0)
                GUILayout.Label(FormatSize((ulong)_strippingSizes[entity]), GUIStyleUtilities.SizeStyle, GUILayout.Width(100));

            GUILayout.EndHorizontal();

            if (!_strippingReasonsFoldout.ContainsKey(entity) || !_strippingReasonsFoldout[entity])
                return;

            EditorGUI.indentLevel++;
            foreach (var reason in reasons)
                StrippingEntityGui(reason, ref odd);
            EditorGUI.indentLevel--;
        }
        
        private static Texture StrippingEntityIcon(string iconString)
        {
            if (IconCache.ContainsKey(iconString))
                return IconCache[iconString];

            if (iconString.StartsWith("class/"))
            {
                var type = System.Type.GetType("UnityEngine." + iconString.Substring(6) + ",UnityEngine");
                if (type != null)
                {
                    var image = EditorGUIUtility.ObjectContent(null, System.Type.GetType("UnityEngine." + iconString.Substring(6) + ",UnityEngine")).image;
                    if (image != null)
                        IconCache[iconString] = image;
                }
            }
            if (iconString.StartsWith("package/"))
            {
                var path = EditorApplication.applicationContentsPath + "/Resources/PackageManager/Editor/" + iconString.Substring(8) + "/.icon.png";
                if (File.Exists(path))
                {
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(File.ReadAllBytes(path));
                    IconCache[iconString] = tex;
                }
            }

            if (!IconCache.ContainsKey(iconString))
                IconCache[iconString] = EditorGUIUtility.ObjectContent(null, typeof(DefaultAsset)).image;

            return IconCache[iconString];
        }
    }
}