using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UniWork.BuildReportInspector.Editor
{
    public partial class BuildReportInspector
    {
        private enum SourceAssetsDisplayMode
        {
            Size,
            OutputDataFiles,
            ImporterType
        };
        
        private struct AssetEntry
        {
            public string name;             // 原文件名称
            public string path;             // 原文件相对路径
            public int size;                // 资源大小 单位: B
            public string outputFile;       // 输出的资源包名
            public string type;             // Importer Type Name
            public Texture icon;            // 资源图标

            public GUIContent buttonGUIContent;
        }
        
        private SourceAssetsDisplayMode _sourceDisplayMode;

        private readonly Dictionary<string, bool> _assetsFoldout = new Dictionary<string, bool>();
        private List<AssetEntry> _assets;
        private Dictionary<string, int> _outputFiles;
        private Dictionary<string, int> _assetTypes;
        
        private void OnAssetsGUI()
        {
            if (_mode == ReportDisplayMode.SourceAssets)
            {
                _sourceDisplayMode = (SourceAssetsDisplayMode)EditorGUILayout.EnumPopup("Sort by:", _sourceDisplayMode);
            }
            
            if (_assets == null)
            {
                _assets = new List<AssetEntry>();
                _outputFiles = new Dictionary<string, int>();
                _assetTypes = new Dictionary<string, int>();
                
                foreach (PackedAssets packedAsset in Report.packedAssets)
                {
                    _outputFiles[packedAsset.shortPath] = 0;
                    ulong totalSizeProp = packedAsset.overhead;
                    _outputFiles[packedAsset.shortPath] = (int)totalSizeProp;
                    
                    foreach (PackedAssetInfo entry in packedAsset.contents)
                    {
                        AssetImporter asset = AssetImporter.GetAtPath(entry.sourceAssetPath);
                        string type = asset != null ? asset.GetType().Name : "Unknown";
                        if (type.EndsWith("Importer"))
                            type = type.Substring(0, type.Length - 8);
                        
                        ulong sizeProp = entry.packedSize;
                        if (!_assetTypes.ContainsKey(type))
                            _assetTypes[type] = 0;
                        
                        _outputFiles[packedAsset.shortPath] += (int)sizeProp;
                        _assetTypes[type] += (int)sizeProp;

                        AssetEntry assetEntry = new AssetEntry
                        {
                            name = string.IsNullOrEmpty(entry.sourceAssetPath)
                                ? "Unknown"
                                : Path.GetFileName(entry.sourceAssetPath),
                            size = (int)sizeProp,
                            icon = AssetDatabase.GetCachedIcon(entry.sourceAssetPath),
                            outputFile = packedAsset.shortPath,
                            type = type,
                            path = entry.sourceAssetPath
                        };
                        assetEntry.buttonGUIContent = new GUIContent(assetEntry.name, assetEntry.path);
                        _assets.Add(assetEntry);
                    }
                }
                
                _assets = _assets.OrderBy(p => -p.size).ToList();
                _outputFiles = _outputFiles.OrderBy(p => -p.Value).ToDictionary(x => x.Key, x => x.Value);
                _assetTypes = _assetTypes.OrderBy(p => -p.Value).ToDictionary(x => x.Key, x => x.Value);
            }
            
            DisplayAssetsView();
        }
        
        private void DisplayAssetsView()
        {
            float vPos = -_scrollPosition.y;

            switch (_sourceDisplayMode)
            {
                case SourceAssetsDisplayMode.Size:
                    ShowAssets(_assets, ref vPos);
                    break;
                case SourceAssetsDisplayMode.OutputDataFiles:
                    foreach (var outputFile in _outputFiles)
                    {
                        if (!_assetsFoldout.ContainsKey(outputFile.Key))
                            _assetsFoldout[outputFile.Key] = false;

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        _assetsFoldout[outputFile.Key] = EditorGUILayout.Foldout(_assetsFoldout[outputFile.Key],
                            outputFile.Key, GUIStyleUtilities.DataFileStyle);
                        GUILayout.Label(FormatSize((ulong)outputFile.Value), GUIStyleUtilities.SizeStyle);
                        GUILayout.EndHorizontal();

                        vPos += LineHeight;

                        if (_assetsFoldout[outputFile.Key])
                            ShowAssets(_assets, ref vPos, outputFile.Key);
                    }
                    break;
                case SourceAssetsDisplayMode.ImporterType:
                    foreach (var outputFile in _assetTypes)
                    {
                        if (!_assetsFoldout.ContainsKey(outputFile.Key))
                            _assetsFoldout[outputFile.Key] = false;

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        _assetsFoldout[outputFile.Key] = EditorGUILayout.Foldout(_assetsFoldout[outputFile.Key],
                            outputFile.Key, GUIStyleUtilities.DataFileStyle);
                        GUILayout.Label(FormatSize((ulong)outputFile.Value), GUIStyleUtilities.SizeStyle);
                        GUILayout.EndHorizontal();

                        vPos += LineHeight;

                        if (_assetsFoldout[outputFile.Key])
                            ShowAssets(_assets, ref vPos, null, outputFile.Key);
                    }             
                    break;
            }
        }
        
        private static void ShowAssets(IEnumerable<AssetEntry> assets, ref float vPos, string fileFilter = null, string typeFilter = null)
        {
            GUILayout.BeginVertical();
            bool odd = false;

            var entries = assets.Where(entry => fileFilter == null || fileFilter == entry.outputFile)
                .Where(entry => typeFilter == null || typeFilter == entry.type);
            
            foreach (AssetEntry entry in entries)
            {
                GUILayout.BeginHorizontal(odd ? GUIStyleUtilities.OddStyle : GUIStyleUtilities.EvenStyle);
                {
                    GUILayout.Label(entry.icon, GUILayout.MaxHeight(16), GUILayout.Width(20));
                    
                    if (GUILayout.Button(entry.buttonGUIContent, GUI.skin.label,
                            GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 110)))
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(entry.path));
                    }
                    
                    GUILayout.Label(FormatSize((ulong)entry.size), GUIStyleUtilities.SizeStyle);
                }
                GUILayout.EndHorizontal();
                
                vPos += LineHeight;
                odd = !odd;
            }

            GUILayout.EndVertical();
        }
    }
}