using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Object = UnityEngine.Object;

namespace Unity.BuildReportInspector
{
    [CustomEditor(typeof(BuildReport))]
    public partial class BuildReportInspector : Editor
    {
        [MenuItem("Window/Open Last Build Report", true)]
        public static bool ValidateOpenLastBuild()
        {
            return File.Exists("Library/LastBuild.buildreport");
        }

        [MenuItem("Window/Open Last Build Report")]
        public static void OpenLastBuild()
        {
            const string buildReportDir = "Assets/BuildReports";
            if (!Directory.Exists(buildReportDir))
                Directory.CreateDirectory(buildReportDir);

            var date = File.GetLastWriteTime("Library/LastBuild.buildreport");
            var assetPath = buildReportDir + "/Build_" + date.ToString("yyyy-dd-MMM-HH-mm-ss") + ".buildreport";
            File.Copy("Library/LastBuild.buildreport", assetPath, true);
            AssetDatabase.ImportAsset(assetPath);
            Selection.objects = new Object[] { AssetDatabase.LoadAssetAtPath<BuildReport>(assetPath) };
        }

        private BuildReport Report => target as BuildReport;

        private const int LineHeight = 20;

        private enum ReportDisplayMode
        {
            BuildSteps,
            SourceAssets,
            OutputFiles,
            Stripping,
            ScenesUsingAssets,
        };

        private readonly string[] _reportDisplayModeStrings =
        {
            ReportDisplayMode.BuildSteps.ToString(),
            ReportDisplayMode.SourceAssets.ToString(),
            ReportDisplayMode.OutputFiles.ToString(),
            ReportDisplayMode.Stripping.ToString(),
            ReportDisplayMode.ScenesUsingAssets.ToString(),
        };

        private ReportDisplayMode _mode;
        private Vector2 _scrollPosition;

        public override void OnInspectorGUI()
        {
            if (Report == null)
            {
                EditorGUILayout.HelpBox("No Build Report.", MessageType.Info);
                return;
            }

            OnTopGUI();
            
            _mode = (ReportDisplayMode)GUILayout.Toolbar((int)_mode, _reportDisplayModeStrings);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            switch(_mode)
            {
                case ReportDisplayMode.BuildSteps:
                    OnBuildStepGUI();
                    break;
                case ReportDisplayMode.SourceAssets:
                    OnAssetsGUI();
                    break;
                case ReportDisplayMode.OutputFiles:
                    OnOutputFilesGUI();
                    break;
                case ReportDisplayMode.Stripping:
                    OnStrippingGUI();
                    break;
                case ReportDisplayMode.ScenesUsingAssets:
                    OnScenesUsingAssetsGUI();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            EditorGUILayout.EndScrollView();
        }

        private void OnTopGUI()
        {
            EditorGUILayout.LabelField("Report Info");
            EditorGUILayout.LabelField("    Build Name: ", Application.productName);
            EditorGUILayout.LabelField("    Platform: ", Report.summary.platform.ToString());
            EditorGUILayout.LabelField("    Total Time: ", FormatTime(Report.summary.totalTime));
            EditorGUILayout.LabelField("    Total Size: ", FormatSize(Report.summary.totalSize));
            EditorGUILayout.LabelField("    Build Result: ", Report.summary.result.ToString());
        }

        private static string FormatSize(ulong size)
        {
            if (size < 1024)
                return size + " B";
            if (size < 1024*1024)
                return (size/1024.00).ToString("F2") + " KB";
            if (size < 1024 * 1024 * 1024)
                return (size / (1024.0 * 1024.0)).ToString("F2") + " MB";
            return (size / (1024.0 * 1024.0 * 1024.0)).ToString("F2") + " GB";
        }
        
        private static string FormatTime(TimeSpan t)
        {
            return t.Hours + ":" + t.Minutes.ToString("D2") + ":" + t.Seconds.ToString("D2") + "." +
                   t.Milliseconds.ToString("D3");
        }
    }
}
