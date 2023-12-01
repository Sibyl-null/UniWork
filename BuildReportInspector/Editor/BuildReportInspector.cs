using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Object = UnityEngine.Object;
using Unity.BuildReportInspector.Mobile;

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
        private MobileAppendix MobileAppendix => MobileHelper.LoadMobileAppendix(Report.summary.guid.ToString());

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
                    OnOutputFilesHeaderGUI();
                    if (MobileAppendix == null)
                        OnOutputFilesGUI();
                    else
                        OnMobileOutputFilesGUI();
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
            EditorGUILayout.LabelField("    Total Size: ", FormatSize(MobileAppendix == null ? Report.summary.totalSize : (ulong)MobileAppendix.BuildSize));
            EditorGUILayout.LabelField("    Build Result: ", Report.summary.result.ToString());

            // Show Mobile appendix data below the build summary
            OnMobileAppendixGUI();
        }

        private void OnMobileAppendixGUI()
        {
            if (MobileAppendix != null)
            {
                if (MobileAppendix.Architectures != null)
                {
                    EditorGUILayout.LabelField("    Download Sizes: ");
                    foreach (var entry in MobileAppendix.Architectures)
                    {
                        var sizeText = entry.DownloadSize == 0 ? "N/A" : FormatSize((ulong) entry.DownloadSize);
                        EditorGUILayout.LabelField($"            {entry.Name}", sizeText);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Could not determine the architectures present in the build.", MessageType.Warning);
                }
            }
#if UNITY_EDITOR_OSX
            // On macOS, show a help dialog for generating the MobileAppendix for iOS/tvOS
            else if (Report.summary.platform == BuildTarget.iOS || Report.summary.platform == BuildTarget.tvOS)
            {
                EditorGUILayout.HelpBox("To get more accurate report data, please provide an .ipa file generated from a " +
                                        "matching Unity build using the dialog below.", MessageType.Warning);
                if (!GUILayout.Button("Select an .ipa bundle"))
                {
                    return;
                }
                var ipaPath = EditorUtility.OpenFilePanel("Select an .ipa build.", "", "ipa");
                if (!string.IsNullOrEmpty(ipaPath))
                {
                    // If an .ipa is selected, generate the MobileAppendix
                    MobileHelper.GenerateAppleAppendix(ipaPath, Report.summary.guid.ToString());
                }
            }
#endif // UNITY_EDITOR_OSX
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
