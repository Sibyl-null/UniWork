using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Unity.BuildReportInspector
{
    public static class BuildReportMenuItem
    {
        private const string BuildReportDir = "Assets/BuildReports";
        private const string TargetReportPath = "Library/LastBuild.buildreport";
        
        [MenuItem("Window/Open Last Build Report", true)]
        public static bool ValidateOpenLastBuild()
        {
            return File.Exists(TargetReportPath);
        }

        [MenuItem("Window/Open Last Build Report")]
        public static void OpenLastBuild()
        {
            if (!Directory.Exists(BuildReportDir))
                Directory.CreateDirectory(BuildReportDir);

            DateTime date = File.GetLastWriteTime(TargetReportPath);
            string assetPath = $"{BuildReportDir}/Build_{date:yyyy-MMM-dd-HH:mm}.buildreport";
            File.Copy(TargetReportPath, assetPath, true);
            
            AssetDatabase.ImportAsset(assetPath);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<BuildReport>(assetPath));
        }
    }
}