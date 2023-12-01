using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniWork.BuildReportInspector.Editor
{
    public partial class BuildReportInspector
    {
        class ScenesUsingAssetGUI
        {
            public string assetPath;
            public string[] scenePaths;
            public bool foldoutState;
        }

        private readonly List<ScenesUsingAssetGUI> _scenesUsingAssetGuIs = new List<ScenesUsingAssetGUI>();
        
        private void OnScenesUsingAssetsGUI()
        {
            if (Report.scenesUsingAssets == null || Report.scenesUsingAssets.Length==0 || Report.scenesUsingAssets[0] == null || Report.scenesUsingAssets[0].list==null || Report.scenesUsingAssets[0].list.Length==0 )
            {
                EditorGUILayout.HelpBox("No info about which scenes are using assets in the build. Did you use BuildOptions.DetailedBuildReport?", MessageType.Info);
                return;
            }

            // re-create list of scenes using assets
            if(!_scenesUsingAssetGuIs.Any())
            {
                foreach (var scenesUsingAsset in Report.scenesUsingAssets[0].list)
                    _scenesUsingAssetGuIs.Add(new ScenesUsingAssetGUI { assetPath = scenesUsingAsset.assetPath, scenePaths = scenesUsingAsset.scenePaths, foldoutState = true});
            }

            bool odd = true;
            foreach (var scenesUsingAssetGUI in _scenesUsingAssetGuIs)
            {
                odd = !odd;
                GUILayout.BeginVertical(odd ? GUIStyleUtilities.OddStyle : GUIStyleUtilities.EvenStyle);

                GUILayout.BeginHorizontal(odd ? GUIStyleUtilities.OddStyle : GUIStyleUtilities.EvenStyle);
                GUILayout.Space(10);
                scenesUsingAssetGUI.foldoutState = EditorGUILayout.Foldout(scenesUsingAssetGUI.foldoutState, scenesUsingAssetGUI.assetPath);
                GUILayout.EndHorizontal();

                if(scenesUsingAssetGUI.foldoutState)
                {
                    foreach (var scenePath in scenesUsingAssetGUI.scenePaths)
                    {
                        odd = !odd;
                        GUILayout.BeginHorizontal(odd ? GUIStyleUtilities.OddStyle : GUIStyleUtilities.EvenStyle);
                        GUILayout.Space(20);
                        GUILayout.Label(scenePath);
                        GUILayout.EndHorizontal();
                    }
                }
                
                GUILayout.EndVertical();
            }
        }
    }
}