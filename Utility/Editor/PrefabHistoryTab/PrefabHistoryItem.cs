using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniWork.Utility.Editor.PrefabHistoryTab
{
    public class PrefabHistoryItem : VisualElement
    {
        private const int MaxLabelLength = 15;
        
        private readonly Button _btnItem;
        private readonly string _prefabPrefabPath;

        public string PrefabPath => _prefabPrefabPath;
        public VisualElement VisualElement => _btnItem;
        
        public PrefabHistoryItem(string prefabPrefabPath)
        {
            _prefabPrefabPath = prefabPrefabPath;

            VisualTreeAsset treeAsset =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/com.sibyl.uniwork/Utility/Editor/PrefabHistoryTab/PrefabHistoryItem.uxml");
            _btnItem = treeAsset.CloneTree().Q<Button>("Item");

            Button btnClose = VisualElement.Q<Button>("Close");
            Label labelName = VisualElement.Q<Label>("Label");

            labelName.text = GetPrefabName();
            
            _btnItem.clicked += OnItemClick;
            btnClose.clicked += OnCloseClick;
            
            _btnItem.RegisterCallback<MouseEnterEvent>(OnHoverChange);
            _btnItem.RegisterCallback<MouseLeaveEvent>(OnHoverChange);
        }

        private string GetPrefabName()
        {
            string prefabName = Path.GetFileNameWithoutExtension(_prefabPrefabPath);

            if (prefabName.Length > MaxLabelLength)
                prefabName = prefabName.Substring(0, MaxLabelLength) + "...";
            
            return prefabName;
        }

        private void OnItemClick()
        {
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabPrefabPath);
            if (prefabAsset == null)
            {
                Debug.LogError("[PrefabHistory] 文件不存在 " + _prefabPrefabPath);
                return;
            }
            
            AssetDatabase.OpenAsset(prefabAsset);
        }
        
        private void OnCloseClick()
        {
            PrefabHistoryTab.RemoveItem(this);
        }
        
        private void OnHoverChange(EventBase e)
        {
            if (PrefabHistoryTab.NowOpenItem == this)
                return;

            if (e.eventTypeId == MouseEnterEvent.TypeId())
            {
                VisualElement.style.backgroundColor = new Color(70f / 255, 70f / 255, 70f / 255, 1);
            }
            else if (e.eventTypeId == MouseLeaveEvent.TypeId())
            {
                VisualElement.style.backgroundColor = new Color(50f / 255, 50f / 255, 50f / 255, 1);
            }
        }

        public void OnOpenChange(bool open)
        {
            VisualElement.style.backgroundColor = open
                ? new Color(95f / 255, 95f / 255, 95f / 255, 1)
                : new Color(50f / 255, 50f / 255, 50f / 255, 1);
        }
    }
}