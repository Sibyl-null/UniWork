using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace UniWork.BuildReportInspector.Editor
{
    public static class BuildReportMenuItem
    {
        private const string BuildReportDir = "Assets/BuildReports";
        private const string TargetReportPath = "Library/LastBuild.buildreport";
        
        [MenuItem("UniWork/EditorTool/Copy Last Build Report", true)]
        public static bool ValidateOpenLastBuild()
        {
            return File.Exists(TargetReportPath);
        }

        [MenuItem("UniWork/EditorTool/Copy Last Build Report")]
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