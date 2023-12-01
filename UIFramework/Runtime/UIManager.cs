using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniWork.UIFramework.Runtime.Scheduler;
using UniWork.Utility.Runtime;

namespace UniWork.UIFramework.Runtime
{
    public partial class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        private static UIManagerBaseAgent _agent;

        private readonly Dictionary<UIEnumBaseType, UIInfo> _infos = 
            new Dictionary<UIEnumBaseType, UIInfo>();
        private readonly Dictionary<UIEnumBaseType, UIBaseCtrl> _instantiatedCtrls =
            new Dictionary<UIEnumBaseType, UIBaseCtrl>();
        private readonly Dictionary<int, RectTransform> _bucketTrans = 
            new Dictionary<int, RectTransform>();

        private readonly Dictionary<UIScheduleMode, UIBaseScheduler> _schedulers = 
            new Dictionary<UIScheduleMode, UIBaseScheduler>()
        {
            { UIScheduleMode.Normal, new UINormalScheduler() },
            { UIScheduleMode.Queue, new UIQueueScheduler() },
            { UIScheduleMode.Stack, new UIStackScheduler() }
        };

        private EventSystem _eventSystem;
        
        public event Action OnEscapeEvent;
        public Camera UICamera { get; private set; }
        public int OrderLayerIncrement { get; private set; } = 0;

        public bool EnableInput
        {
            get => _eventSystem.isActiveAndEnabled;
            set => _eventSystem.enabled = value;
        }

        public static void Create(UIManagerBaseAgent agent)
        {
            if (Instance != null)
                throw new Exception("UIManager repeat created");
            
            _agent = agent;
            GameObject obj = Instantiate(_agent.Load<GameObject>(_agent.UIRootLoadPath));
            DontDestroyOnLoad(obj);
            
            Instance = obj.GetOrAddComponent<UIManager>();
            Instance.Initialize();
        }

        private void Initialize()
        {
            UICamera = GetComponentInChildren<Camera>();
            _eventSystem = GetComponentInChildren<EventSystem>();
            _agent.InitUIInfo();
            CreateBuckets();
        }

        internal void AddInfo(UIEnumBaseType uiEnumType, UIInfo info)
        {
            if (_infos.ContainsKey(uiEnumType))
                throw new Exception($"{uiEnumType}已注册");
            
            _infos.Add(uiEnumType, info);
        }

        private void CreateBuckets()
        {
            var layers = _agent.GetAllLayers();
            foreach (UIEnumBaseLayer baseLayer in layers)
            {
                GameObject bucketObj = new GameObject(baseLayer.value);
                bucketObj.layer = LayerMask.NameToLayer("UI");
                bucketObj.transform.SetParent(this.transform, false);

                RectTransform rectTrans = bucketObj.GetOrAddComponent<RectTransform>();
                rectTrans.Overspread();

                _bucketTrans.Add(baseLayer.key, rectTrans);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && EnableInput)
            {
                UIStackScheduler stackScheduler = (UIStackScheduler)_schedulers[UIScheduleMode.Stack];
                if (!stackScheduler.IsEmpty)
                {
                    stackScheduler.EscapeUI();
                }
                else
                    OnEscapeEvent?.Invoke();
            }
        }

        
        // ----------------------------------------------------------------------------
        // 对外API
        // ----------------------------------------------------------------------------

        public void ShowUI(UIEnumBaseType uiEnumType, UIBaseParameter param = null)
        {
            UIInfo info = GetUIInfo(uiEnumType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                scheduler.ShowUI(uiEnumType, param);
            else
                DLog.Error($"不存在{info.ScheduleMode}类型的UI调度器");
        }
        
        public void HideUI(UIEnumBaseType uiEnumType)
        {
            UIInfo info = GetUIInfo(uiEnumType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                scheduler.HideUI(uiEnumType);
            else
                DLog.Error($"不存在{info.ScheduleMode}类型的UI调度器");
        }
        
        public void DestroyUI(UIEnumBaseType uiEnumType)
        {
            UIInfo info = GetUIInfo(uiEnumType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                scheduler.DestroyUI(uiEnumType);
            else
                DLog.Error($"不存在{info.ScheduleMode}类型的UI调度器");
        }
        
        internal void ShowUIInternal(UIEnumBaseType uiEnumType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = GetUICtrl(uiEnumType);

            OrderLayerIncrement += _agent.LayerOrderOnceRaise;
            
            if (ctrl == null)
            {
                UIInfo info = _infos[uiEnumType];
                GameObject uiObj = CreateUIObject(info);
                ctrl = CreateUICtrl(uiObj, info);
                ctrl.OnShow(param);
                return;
            }

            if (!ctrl.IsShow)
                ctrl.OnShow(param);
        }

        internal void HideUIInternal(UIEnumBaseType uiEnumType)
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

        internal void DestroyUIInternal(UIEnumBaseType uiEnumType)
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
            Destroy(ctrl.UIView.gameObject);
            
            _agent.UnLoad(ctrl.Info.ResPath);
            _instantiatedCtrls.Remove(uiEnumType);
        }

        public UIBaseCtrl GetUICtrl(UIEnumBaseType uiEnumType)
        {
            if (!_infos.ContainsKey(uiEnumType))
                throw new Exception(uiEnumType + "对应的UIInfo不存在");
            
            return _instantiatedCtrls.TryGetValue(uiEnumType, out UIBaseCtrl ctrl) ? ctrl : null;
        }

        public UIInfo GetUIInfo(UIEnumBaseType uiEnumType)
        {
            if (!_infos.ContainsKey(uiEnumType))
                throw new Exception(uiEnumType + "对应的UIInfo不存在");

            return _infos[uiEnumType];
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