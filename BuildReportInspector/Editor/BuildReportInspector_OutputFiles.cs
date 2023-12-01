using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.BuildReportInspector
{
    public partial class BuildReportInspector
    {
        private void OnOutputFilesHeaderGUI()
        {
            if (_mode == ReportDisplayMode.OutputFiles && MobileAppendix != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("File"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 260));
                GUILayout.Label("Uncompressed size", GUIStyleUtilities.SizeStyle);
                GUILayout.Label("Compressed size", GUIStyleUtilities.SizeStyle);
                GUILayout.EndHorizontal();
            } 
        }
        
        private void OnOutputFilesGUI()
        {
            if (Report.files.Length == 0)
                return;

            var longestCommonRoot = Report.files[0].path;
            var tempRoot = Path.GetFullPath("Temp");
            foreach (var file in Report.files)
            {
                if (file.path.StartsWith(tempRoot))
                    continue;
                for (var i = 0; i < longestCommonRoot.Length && i < file.path.Length; i++)
                {
                    if (longestCommonRoot[i] == file.path[i])
                        continue;
                    longestCommonRoot = longestCommonRoot.Substring(0, i);
                    break;
                }
            }
            var odd = false;
            foreach (var file in Report.files)
            {
                if (file.path.StartsWith(tempRoot))
                    continue;
                GUILayout.BeginHorizontal(odd ? GUIStyleUtilities.OddStyle : GUIStyleUtilities.EvenStyle);
                odd = !odd;
                GUILayout.Label(new GUIContent(file.path.Substring(longestCommonRoot.Length), file.path), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 260));
                GUILayout.Label(file.role);
                GUILayout.Label(FormatSize(file.size), GUIStyleUtilities.SizeStyle);
                GUILayout.EndHorizontal();

            }
        }

        private void OnMobileOutputFilesGUI()
        {
            var longestCommonRoot = MobileAppendix.Files[0].Path;
            var tempRoot = Path.GetFullPath("Temp");
            foreach (var file in MobileAppendix.Files)
            {
                if (file.Path.StartsWith(tempRoot))
                    continue;
                for (var i = 0; i < longestCommonRoot.Length && i < file.Path.Length; i++)
                {
                    if (longestCommonRoot[i] == file.Path[i])
                        continue;
                    longestCommonRoot = longestCommonRoot.Substring(0, i);
                    break;
                }
            }
            var odd = false;
            foreach (var file in MobileAppendix.Files)
            {
                if (file.Path.StartsWith(tempRoot))
                    continue;
                GUILayout.BeginHorizontal(odd ? GUIStyleUtilities.OddStyle : GUIStyleUtilities.EvenStyle);
                odd = !odd;
                GUILayout.Label(new GUIContent(file.Path.Substring(longestCommonRoot.Length), file.Path), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 260));
                GUILayout.Label(FormatSize((ulong)file.UncompressedSize), GUIStyleUtilities.SizeStyle);
                GUILayout.Label(FormatSize((ulong)file.CompressedSize), GUIStyleUtilities.SizeStyle);
                GUILayout.EndHorizontal();
            }
        }   
    }
}