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

        private readonly Dictionary<Type, UIInfo> _infoDic = new();
        private readonly Dictionary<Type, UIBaseCtrl> _instantiatedCtrlDic = new();
        private readonly Dictionary<string, Canvas> _bucketCanvasDic = new();

        private readonly Dictionary<UIScheduleMode, UIBaseScheduler> _schedulers = new()
        {
            { UIScheduleMode.Normal, new UINormalScheduler() },
            { UIScheduleMode.Queue, new UIQueueScheduler() },
            { UIScheduleMode.Stack, new UIStackScheduler() }
        };

        private UIBaseAgent _agent;
        private GameObject _rootGo;
        private EventSystem _eventSystem;
        private UIRuntimeSetting _runtimeSetting;
        private int _orderLayerIncrement;

        public event Action OnEscapeEvent;
        public Camera UICamera { get; private set; }

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
            
            Instance = new UIManager();
            Instance.Initialize(agent);
        }

        private void Initialize(UIBaseAgent agent)
        {
            _agent = agent;
            _runtimeSetting = _agent.Load<UIRuntimeSetting>(_agent.RuntimeSettingLoadPath);

            _rootGo = Object.Instantiate(_runtimeSetting.rootPrefab);
            Object.DontDestroyOnLoad(_rootGo);
            
            UICamera = _rootGo.GetComponentInChildren<Camera>();
            _eventSystem = _rootGo.GetComponentInChildren<EventSystem>();
            
            _agent.InitUIInfo();
            CreateBuckets();
        }

        internal void AddInfo(Type ctrlType, UIInfo info)
        {
            if (_infoDic.ContainsKey(ctrlType))
                throw new Exception($"[UIFramework] {ctrlType.Name} 已注册");
            
            _infoDic.Add(ctrlType, info);
        }

        private void CreateBuckets()
        {
            foreach (UIRuntimeSetting.ShowLayer layer in _runtimeSetting.showLayers)
            {
                GameObject bucketObj = new GameObject(layer.name);
                bucketObj.layer = LayerMask.NameToLayer("UI");
                bucketObj.transform.SetParent(_rootGo.transform, false);

                RectTransform rectTrans = bucketObj.GetOrAddComponent<RectTransform>();
                rectTrans.Overspread();

                Canvas canvas = bucketObj.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = layer.order;

                _bucketCanvasDic.Add(layer.name, canvas);
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

        public void ShowUI<T>(UIBaseParameter param = null) where T : UIBaseCtrl
        {
            Type ctrlType = typeof(T);
            ShowUI(ctrlType, param);
        }

        public async UniTask ShowUIAsync<T>(UIBaseParameter param = null) where T : UIBaseCtrl
        {
            Type ctrlType = typeof(T);
            await ShowUIAsync(ctrlType, param);
        }
        
        public void HideUI<T>() where T : UIBaseCtrl
        {
            Type ctrlType = typeof(T);
            HideUI(ctrlType);
        }
        
        public void DestroyUI<T>() where T : UIBaseCtrl
        {
            Type ctrlType = typeof(T);
            DestroyUI(ctrlType);
        }
        
        public void ShowUI(Type ctrlType, UIBaseParameter param = null)
        {
            UIInfo info = GetUIInfo(ctrlType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                scheduler.ShowUI(ctrlType, param);
            else
                DLog.Error($"[UIFramework] 不存在 {info.ScheduleMode} 类型的调度器");
        }

        public async UniTask ShowUIAsync(Type ctrlType, UIBaseParameter param = null)
        {
            UIInfo info = GetUIInfo(ctrlType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                await scheduler.ShowUIAsync(ctrlType, param);
            else
                DLog.Error($"[UIFramework] 不存在 {info.ScheduleMode} 类型的调度器");
        }
        
        public void HideUI(Type ctrlType)
        {
            UIInfo info = GetUIInfo(ctrlType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                scheduler.HideUI(ctrlType);
            else
                DLog.Error($"[UIFramework] 不存在 {info.ScheduleMode} 类型的调度器");
        }
        
        public void DestroyUI(Type ctrlType)
        {
            UIInfo info = GetUIInfo(ctrlType);

            if (_schedulers.TryGetValue(info.ScheduleMode, out UIBaseScheduler scheduler))
                scheduler.DestroyUI(ctrlType);
            else
                DLog.Error($"[UIFramework] 不存在 {info.ScheduleMode} 类型的调度器");
        }
        
        internal void ShowUIInternal(Type ctrlType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = GetUICtrl(ctrlType);

            _orderLayerIncrement += _runtimeSetting.layerOrderOnceRaise;
            
            if (ctrl == null)
            {
                UIInfo info = _infoDic[ctrlType];
                GameObject uiObj = CreateUIObject(info);
                ctrl = CreateUICtrl(uiObj, ctrlType);
                ctrl.OnShow(param);
                return;
            }

            if (!ctrl.IsShow)
                ctrl.OnShow(param);
        }

        internal async UniTask ShowUIAsyncInternal(Type ctrlType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = GetUICtrl(ctrlType);

            _orderLayerIncrement += _runtimeSetting.layerOrderOnceRaise;
            
            if (ctrl == null)
            {
                UIInfo info = _infoDic[ctrlType];
                GameObject uiObj = await CreateUIObjectAsync(info);
                ctrl = CreateUICtrl(uiObj, ctrlType);
                ctrl.OnShow(param);
                return;
            }

            if (!ctrl.IsShow)
                ctrl.OnShow(param);
        }

        internal void HideUIInternal(Type ctrlType)
        {
            UIBaseCtrl ctrl = GetUICtrl(ctrlType);
            
            if (ctrl == null)
            {
                DLog.Warning($"[UIFramework] {ctrlType.Name} 未实例化");
                return;
            }
            
            if (ctrl.IsShow)
                ctrl.OnHide();
        }

        internal void DestroyUIInternal(Type ctrlType)
        {
            UIBaseCtrl ctrl = GetUICtrl(ctrlType);
            
            if (ctrl == null)
            {
                DLog.Warning($"[UIFramework] {ctrlType.Name} 未实例化");
                return;
            }
            
            if (ctrl.IsShow)
                ctrl.OnHide();
            
            ctrl.OnDestroy();
            Object.Destroy(ctrl.UIView.gameObject);
            
            _agent.UnLoad(ctrl.Info.ResPath);
            _instantiatedCtrlDic.Remove(ctrlType);
        }

        public UIBaseCtrl GetUICtrl(Type ctrlType)
        {
            if (!_infoDic.ContainsKey(ctrlType))
                throw new Exception($"[UIFramework] {ctrlType.Name} 对应的 UIInfo 不存在");
            
            return _instantiatedCtrlDic.TryGetValue(ctrlType, out UIBaseCtrl ctrl) ? ctrl : null;
        }

        private UIInfo GetUIInfo(Type ctrlType)
        {
            if (!_infoDic.ContainsKey(ctrlType))
                throw new Exception($"[UIFramework] {ctrlType.Name} 对应的 UIInfo 不存在");

            return _infoDic[ctrlType];
        }

        private GameObject CreateUIObject(UIInfo info)
        {
            Transform bucketTrans = _bucketCanvasDic[info.LayerName].transform;
            GameObject uiPrefab = _agent.Load<GameObject>(info.ResPath);
            return Object.Instantiate(uiPrefab, bucketTrans, false);
        }

        private async UniTask<GameObject> CreateUIObjectAsync(UIInfo info)
        {
            Transform bucketTrans = _bucketCanvasDic[info.LayerName].transform;
            GameObject uiPrefab = await _agent.LoadAsync<GameObject>(info.ResPath);
            return Object.Instantiate(uiPrefab, bucketTrans, false);
        }

        private UIBaseCtrl CreateUICtrl(GameObject uiObj, Type ctrlType)
        {
            UIInfo info = _infoDic[ctrlType];
            
            UIBaseView view = (UIBaseView)uiObj.GetComponent(typeof(UIBaseView));
            UIBaseCtrl ctrl = (UIBaseCtrl)Activator.CreateInstance(ctrlType);
            ctrl.Initialize(view, info);

            _instantiatedCtrlDic.Add(ctrlType, ctrl);
            return ctrl;
        }

        public int GetLayerOrderWithIncrement(string layerName)
        {
            if (_bucketCanvasDic.TryGetValue(layerName, out Canvas canvas))
            {
                return canvas.sortingOrder + _orderLayerIncrement;
            }

            DLog.Error("[UIFramework] 不存在该层级: " + layerName);
            return 0;
        }
    }
}