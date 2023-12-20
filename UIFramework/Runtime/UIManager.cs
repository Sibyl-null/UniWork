using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UniWork.UIFramework.Runtime.Scheduler;
using UniWork.Utility.Runtime;
using Object = UnityEngine.Object;

namespace UniWork.UIFramework.Runtime
{
    public class UIManager
    {
        public static UIManager Instance { get; private set; }
        private static UIBaseAgent _agent;

        private readonly Dictionary<UIBaseType, UIInfo> _infos = new();
        private readonly Dictionary<UIBaseType, UIBaseCtrl> _instantiatedCtrls = new();
        private readonly Dictionary<int, RectTransform> _bucketTrans = new();

        private readonly Dictionary<UIScheduleMode, UIBaseScheduler> _schedulers = new()
        {
            { UIScheduleMode.Normal, new UINormalScheduler() },
            { UIScheduleMode.Queue, new UIQueueScheduler() },
            { UIScheduleMode.Stack, new UIStackScheduler() }
        };

        private GameObject _rootGo;
        private EventSystem _eventSystem;
        
        public event Action OnEscapeEvent;
        public Camera UICamera { get; private set; }
        public int OrderLayerIncrement { get; private set; }

        public bool EnableInput
        {
            get => _eventSystem.isActiveAndEnabled;
            set => _eventSystem.enabled = value;
        }
        
        private UIManager(){}

        public static void Create(UIBaseAgent agent)
        {
            if (Instance != null)
                throw new Exception("UIManager repeat created");
            
            _agent = agent;
            GameObject obj = Object.Instantiate(_agent.Load<GameObject>(_agent.UIRootLoadPath));
            Object.DontDestroyOnLoad(obj);

            Instance = new UIManager();
            Instance.Initialize();
        }

        private void Initialize()
        {
            UICamera = _rootGo.GetComponentInChildren<Camera>();
            _eventSystem = _rootGo.GetComponentInChildren<EventSystem>();
            _agent.InitUIInfo();
            CreateBuckets();
        }

        internal void AddInfo(UIInfo info)
        {
            if (_infos.ContainsKey(info.UIBaseType))
                throw new Exception($"{info.UIBaseType}已注册");
            
            _infos.Add(info.UIBaseType, info);
        }

        private void CreateBuckets()
        {
            var layers = _agent.GetAllLayers();
            foreach (UIBaseLayer baseLayer in layers)
            {
                GameObject bucketObj = new GameObject(baseLayer.value);
                bucketObj.layer = LayerMask.NameToLayer("UI");
                bucketObj.transform.SetParent(_rootGo.transform, false);

                RectTransform rectTrans = bucketObj.GetOrAddComponent<RectTransform>();
                rectTrans.Overspread();

                _bucketTrans.Add(baseLayer.key, rectTrans);
            }
        }

        /**
         * 需要外部在按下返回键时调用 (例如键盘 ESC 键，手机返回键等)
         */
        public void RunEscapeClick()
        {
            if (EnableInput)
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

        public void ShowUI(UIBaseType uiType, UIBaseParameter param = null)
        {
            UIInfo info = GetUIInfo(uiType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                scheduler.ShowUI(uiType, param);
            else
                DLog.Error($"不存在{info.ScheduleMode}类型的UI调度器");
        }

        public async UniTask ShowUIAsync(UIBaseType uiType, UIBaseParameter param = null)
        {
            UIInfo info = GetUIInfo(uiType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                await scheduler.ShowUIAsync(uiType, param);
            else
                DLog.Error($"不存在{info.ScheduleMode}类型的UI调度器");
        }
        
        public void HideUI(UIBaseType uiType)
        {
            UIInfo info = GetUIInfo(uiType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                scheduler.HideUI(uiType);
            else
                DLog.Error($"不存在{info.ScheduleMode}类型的UI调度器");
        }
        
        public void DestroyUI(UIBaseType uiType)
        {
            UIInfo info = GetUIInfo(uiType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                scheduler.DestroyUI(uiType);
            else
                DLog.Error($"不存在{info.ScheduleMode}类型的UI调度器");
        }
        
        internal void ShowUIInternal(UIBaseType uiType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = GetUICtrl(uiType);

            OrderLayerIncrement += _agent.LayerOrderOnceRaise;
            
            if (ctrl == null)
            {
                UIInfo info = _infos[uiType];
                GameObject uiObj = CreateUIObject(info);
                ctrl = CreateUICtrl(uiObj, info);
                ctrl.OnShow(param);
                return;
            }

            if (!ctrl.IsShow)
                ctrl.OnShow(param);
        }

        internal async UniTask ShowUIAsyncInternal(UIBaseType uiType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = GetUICtrl(uiType);

            OrderLayerIncrement += _agent.LayerOrderOnceRaise;
            
            if (ctrl == null)
            {
                UIInfo info = _infos[uiType];
                GameObject uiObj = await CreateUIObjectAsync(info);
                ctrl = CreateUICtrl(uiObj, info);
                ctrl.OnShow(param);
                return;
            }

            if (!ctrl.IsShow)
                ctrl.OnShow(param);
        }

        internal void HideUIInternal(UIBaseType uiType)
        {
            UIBaseCtrl ctrl = GetUICtrl(uiType);
            
            if (ctrl == null)
            {
                DLog.Warning($"{uiType} 未实例化");
                return;
            }
            
            if (ctrl.IsShow)
                ctrl.OnHide();
        }

        internal void DestroyUIInternal(UIBaseType uiType)
        {
            UIBaseCtrl ctrl = GetUICtrl(uiType);
            
            if (ctrl == null)
            {
                DLog.Warning($"{uiType} 未实例化");
                return;
            }
            
            if (ctrl.IsShow)
                ctrl.OnHide();
            
            ctrl.OnDestroy();
            Object.Destroy(ctrl.UIView.gameObject);
            
            _agent.UnLoad(ctrl.Info.ResPath);
            _instantiatedCtrls.Remove(uiType);
        }

        public UIBaseCtrl GetUICtrl(UIBaseType uiType)
        {
            if (!_infos.ContainsKey(uiType))
                throw new Exception(uiType + "对应的UIInfo不存在");
            
            return _instantiatedCtrls.TryGetValue(uiType, out UIBaseCtrl ctrl) ? ctrl : null;
        }

        public UIInfo GetUIInfo(UIBaseType uiType)
        {
            if (!_infos.ContainsKey(uiType))
                throw new Exception(uiType + "对应的UIInfo不存在");

            return _infos[uiType];
        }

        private GameObject CreateUIObject(UIInfo info)
        {
            Transform bucketTrans = _bucketTrans[info.UIBaseLayer.key];
            GameObject uiPrefab = _agent.Load<GameObject>(info.ResPath);
            return Object.Instantiate(uiPrefab, bucketTrans, false);
        }

        private async UniTask<GameObject> CreateUIObjectAsync(UIInfo info)
        {
            Transform bucketTrans = _bucketTrans[info.UIBaseLayer.key];
            GameObject uiPrefab = await _agent.LoadAsync<GameObject>(info.ResPath);
            return Object.Instantiate(uiPrefab, bucketTrans, false);
        }

        private UIBaseCtrl CreateUICtrl(GameObject uiObj, UIInfo info)
        {
            UIBaseView view = (UIBaseView)uiObj.GetComponent(typeof(UIBaseView));
            UIBaseCtrl ctrl = (UIBaseCtrl)Activator.CreateInstance(info.CtrlType);
            ctrl.Initialize(view, info);

            _instantiatedCtrls.Add(info.UIBaseType, ctrl);
            return ctrl;
        }
    }
}