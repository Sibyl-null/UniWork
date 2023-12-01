using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniWork.Utility.Editor.PrefabHistoryTab
{
    public static class PrefabHistoryTab
    {
        private static PrefabHistoryRecord _record;
        private static VisualElement _panel;
        private static ScrollView _scrollView;
        private static readonly Dictionary<string, PrefabHistoryItem> _itemDic = new();

        public static PrefabHistoryItem NowOpenItem { get; private set; }

        static PrefabHistoryTab()
        {
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            PrefabStage.prefabStageClosing += OnPrefabStageClosing;
        }

        [DidReloadScripts]
        public static void ShowPrefabHistoryTab()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null || _panel != null)
                return;

            if (_record == null)
                _record = PrefabHistoryRecord.LoadRecord();

            VisualTreeAsset treeAsset =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/com.sibyl.uniwork/Utility/Editor/PrefabHistoryTab/PrefabHistoryTab.uxml");
            if (treeAsset == null)
                return;
            
            _panel = treeAsset.CloneTree();
            _scrollView = _panel.Q<ScrollView>("ScrollView");
            
            _panel.contentContainer.RegisterCallback<MouseEnterEvent>(e =>
            {
                _scrollView.horizontalScrollerVisibility = ScrollerVisibility.Auto;
            });
            _panel.contentContainer.RegisterCallback<MouseLeaveEvent>((e) =>
            {
                Vector2 old = _scrollView.scrollOffset;
                _scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                _scrollView.scrollOffset = old;
            });
            
            Scroller scroller = _scrollView.horizontalScroller;
            scroller.Remove(scroller.lowButton);
            scroller.Remove(scroller.highButton);
            scroller.slider.style.height = 5;
            scroller.slider.style.marginLeft = scroller.slider.style.marginRight = 0;
            scroller.style.height = 6;
            
            _itemDic.Clear();
            RefreshScrollView();

            sceneView.rootVisualElement.Add(_panel);
        }

        private static void OnPrefabStageOpened(PrefabStage stage)
        {
            if (_panel == null || _scrollView == null)
                ShowPrefabHistoryTab();
            
            AddItem(stage.assetPath);
            
            NowOpenItem?.OnOpenChange(false);
            NowOpenItem = _itemDic[stage.assetPath];
            NowOpenItem?.OnOpenChange(true);
        }
        
        private static void OnPrefabStageClosing(PrefabStage stage)
        {
            if (NowOpenItem == null || NowOpenItem.PrefabPath != stage.assetPath)
                return;
            
            NowOpenItem.OnOpenChange(false);
            NowOpenItem = null;
        }
        
        private static void AddItem(string path)
        {
            if (_record.HasRecord(path))
                return;
            
            _record.AddRecord(path);
            PrefabHistoryItem item = new PrefabHistoryItem(path);
            _itemDic.Add(path, item);
            RefreshScrollView();
        }
        
        public static void RemoveItem(PrefabHistoryItem item)
        {
            if (_record.HasRecord(item.PrefabPath) == false)
                return;
            
            _record.RemoveRecord(item.PrefabPath);
            _itemDic.Remove(item.PrefabPath);
            RefreshScrollView();
        }

        private static void RefreshScrollView()
        {
            _scrollView.Clear();
            for (int i = _record.PrefabPaths.Count - 1; i >= 0; --i)
            {
                string path = _record.PrefabPaths[i];

                if (_itemDic.ContainsKey(path) == false)
                    _itemDic.Add(path, new PrefabHistoryItem(path));
                
                _scrollView.Add(_itemDic[path].VisualElement);
            }
        }
    }
}