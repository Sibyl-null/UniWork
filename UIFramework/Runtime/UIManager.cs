using System;
using System.Collections.Generic;
using SFramework.Utility;
using UnityEngine;

namespace SFramework.UIFramework.Runtime
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        private static UIManagerBaseAgent _agent;
        
        private readonly Dictionary<UIEnumBaseType, UIInfo> _infos = new Dictionary<UIEnumBaseType, UIInfo>();
        private readonly Dictionary<UIEnumBaseType, UIBaseCtrl> _instantiatedCtrls = new Dictionary<UIEnumBaseType, UIBaseCtrl>();
        private readonly Dictionary<int, Transform> _bucketTrans = new Dictionary<int, Transform>();

        private readonly Queue<UIEnumBaseType> _uiQueue = new Queue<UIEnumBaseType>();
        private readonly Stack<UIEnumBaseType> _uiStack = new Stack<UIEnumBaseType>();

        public static UIManager Instance => _instance;
        public event Action EscapeEvent;
        public Camera UICamera { get; private set; }
        public int OrderLayerIncrement { get; private set; } = 0;

        public static void Create(UIManagerBaseAgent agent)
        {
            if (_instance != null)
                throw new Exception("UIManager repeat created");
            
            _agent = agent;
            GameObject obj = Instantiate(_agent.Load<GameObject>(UIDefine.UIRootPath));
            _instance = obj.GetOrAddComponent<UIManager>();
            _instance.Initialize();
        }

        private void Initialize()
        {
            UICamera = GetComponentInChildren<Camera>();
            _agent.InitUIInfo(AddInfo);
            CreateBuckets();
        }

        private void AddInfo(UIEnumBaseType uiEnumType, UIInfo info)
        {
            if (_infos.ContainsKey(uiEnumType))
                throw new Exception($"{uiEnumType.ToString()}已注册");
            
            _infos.Add(uiEnumType, info);
        }

        private void CreateBuckets()
        {
            var layers = _agent.GetAllLayers();
            foreach (UIEnumBaseLayer baseLayer in layers)
            {
                GameObject bucketObj = new GameObject(baseLayer.value);
                bucketObj.transform.SetParent(this.transform, false);
                _bucketTrans.Add(baseLayer.key, bucketObj.transform);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_uiStack.Count > 0)
                {
                    GetUICtrl(_uiStack.Peek()).OnEscape();
                }
                else
                    EscapeEvent?.Invoke();
            }
        }

        // --------------------------------
        // 对外API
        // --------------------------------

        public void ShowUI(UIEnumBaseType uiEnumType)
        {
            if (!_infos.ContainsKey(uiEnumType))
                throw new Exception(uiEnumType + "对应的UIInfo不存在");

            UIInfo info = _infos[uiEnumType];
            switch (info.ScheduleMode)
            {
                case UIScheduleMode.Normal:
                    ShowUIInternal(uiEnumType);
                    break;
                case UIScheduleMode.Queue:
                    if (_uiQueue.Count == 0)
                        ShowUIInternal(uiEnumType);
                    _uiQueue.Enqueue(uiEnumType);
                    break;
                case UIScheduleMode.Stack:
                    if (_uiStack.Count > 0)
                        HideUIInternal(_uiStack.Peek());
                    _uiStack.Push(uiEnumType);
                    ShowUIInternal(uiEnumType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ShowUIInternal(UIEnumBaseType uiEnumType)
        {
            UIBaseCtrl ctrl = GetUICtrl(uiEnumType);

            if (ctrl == null)
            {
                UIInfo info = _infos[uiEnumType];
                GameObject uiObj = CreateUIObject(info);
                ctrl = CreateUICtrl(uiObj, info);
            }
            
            ++OrderLayerIncrement;
            if (!ctrl.IsShow)
                ctrl.OnShow();
        }

        public void HideUI(UIEnumBaseType uiEnumType)
        {
            if (!_infos.ContainsKey(uiEnumType))
                throw new Exception(uiEnumType + "对应的UIInfo不存在");

            UIInfo info = _infos[uiEnumType];
            switch (info.ScheduleMode)
            {
                case UIScheduleMode.Normal:
                    HideUIInternal(uiEnumType);
                    break;
                case UIScheduleMode.Queue:
                    HideUIInternal(uiEnumType);
                    TryShowNextQueueUI(uiEnumType);
                    break;
                case UIScheduleMode.Stack:
                    HideUIInternal(uiEnumType);
                    TryShowNextStackUI(uiEnumType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HideUIInternal(UIEnumBaseType uiEnumType)
        {
            UIBaseCtrl ctrl = GetUICtrl(uiEnumType);
            
            if (ctrl == null)
            {
                DLog.Warning($"{uiEnumType} 未实例化");
                return;
            }
            
            if (ctrl.IsShow)
                ctrl.OnHide();
        }

        public void DestroyUI(UIEnumBaseType uiEnumType)
        {
            UIBaseCtrl ctrl = GetUICtrl(uiEnumType);
            
            if (ctrl == null)
            {
                DLog.Warning($"{uiEnumType} 未实例化");
                return;
            }
            
            if (ctrl.IsShow)
                ctrl.OnHide();
            
            ctrl.OnDestroy();
            _instantiatedCtrls.Remove(uiEnumType);

            TryShowNextQueueUI(uiEnumType);
            TryShowNextStackUI(uiEnumType);
        }

        private void TryShowNextQueueUI(UIEnumBaseType uiEnumType)
        {
            UIInfo info = _infos[uiEnumType];
            if (info.ScheduleMode == UIScheduleMode.Queue && _uiQueue.Count > 0 && _uiQueue.Peek() == uiEnumType)
            {
                _uiQueue.Dequeue();
                if (_uiQueue.Count > 0)
                    ShowUIInternal(_uiQueue.Peek());
            }
        }

        private void TryShowNextStackUI(UIEnumBaseType uiEnumType)
        {
            UIInfo info = _infos[uiEnumType];
            if (info.ScheduleMode == UIScheduleMode.Stack && _uiStack.Count > 0 && _uiStack.Peek() == uiEnumType)
            {
                _uiStack.Pop();
                if (_uiStack.Count > 0)
                    ShowUIInternal(_uiStack.Peek());
            }
        }
        
        public UIBaseCtrl GetUICtrl(UIEnumBaseType uiEnumType)
        {
            if (!_infos.ContainsKey(uiEnumType))
                throw new Exception(uiEnumType + "对应的UIInfo不存在");
            
            return _instantiatedCtrls.TryGetValue(uiEnumType, out UIBaseCtrl ctrl) ? ctrl : null;
        }

        private GameObject CreateUIObject(UIInfo info)
        {
            Transform bucketTrans = _bucketTrans[info.UIEnumBaseLayer.key];
            GameObject uiPrefab = _agent.Load<GameObject>(info.ResPath);
            return Instantiate(uiPrefab, bucketTrans, false);
        }

        private UIBaseCtrl CreateUICtrl(GameObject uiObj, UIInfo info)
        {
            UIBaseView view = (UIBaseView)uiObj.GetComponent(info.ViewType);
            UIBaseCtrl ctrl = (UIBaseCtrl)Activator.CreateInstance(info.CtrlType);
            ctrl.Initialize(view, info);

            _instantiatedCtrls.Add(info.UIEnumBaseType, ctrl);
            return ctrl;
        }
    }
}