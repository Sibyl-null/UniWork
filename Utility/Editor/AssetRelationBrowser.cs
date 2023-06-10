#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace SFramework.Utility.Editor
{
    public class AssetRelationBrowser : OdinEditorWindow
    {
        [MenuItem("Assets/Browse Relation", false, 0)]
        public static void Open()
        {
            GetWindow<AssetRelationBrowser>().Show();
        }
        
        public Dictionary<string, List<UnityEngine.Object>> dependencies;
        public Dictionary<string, List<UnityEngine.Object>> references;

        protected override void OnEnable()
        {
            base.OnEnable();
            dependencies = new Dictionary<string, List<UnityEngine.Object>>();
            references = new Dictionary<string, List<UnityEngine.Object>>();
            GetDependencies();
            GetReferences();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            dependencies = null;
            references = null;
        }

        private void GetDependencies()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
            string[] paths = AssetDatabase.GetDependencies(new[] { assetPath });

            foreach (string path in paths)
            {
                if (path == assetPath)
                    continue;
                
                Type depType = AssetDatabase.GetMainAssetTypeAtPath(path);
                
                if (dependencies.ContainsKey(depType.Name) == false)
                    dependencies.Add(depType.Name, new List<UnityEngine.Object>());
                
                dependencies[depType.Name].Add(AssetDatabase.LoadMainAssetAtPath(path));
            }
        }

        private void GetReferences()
        {
            string[] checkPaths = new[] { "Assets" };
            string[] checkTypes = new[] { "prefab", "scene" };

            string filter = "";
            foreach (string checkType in checkTypes)
            {
                filter += $"t:{checkType} ";
            }

            string[] guids = AssetDatabase.FindAssets(filter, checkPaths);
            string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (selectionPath == assetPath)
                    continue;

                string[] depPaths = AssetDatabase.GetDependencies(assetPath);
                if (depPaths.Contains(selectionPath))
                {
                    Type refType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                    
                    if (references.ContainsKey(refType.Name) == false)
                        references.Add(refType.Name, new List<UnityEngine.Object>());
                    
                    references[refType.Name].Add(AssetDatabase.LoadMainAssetAtPath(assetPath));
                }
            }
        }
    }
}
#endif