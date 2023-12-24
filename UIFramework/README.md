# UIFramework 使用
## 基本介绍
一个UI面板用一个 Canvas 进行管理，UIBaseView 的子类 View 挂载到 Prefab 上，用于关联需要的组件。 
UIBaseCtrl 的子类持有对应的 View 对象，用于实现交互逻辑。

&nbsp;

每个UI面板都有一个 UIInfo 信息来描述自己。这部分信息通过代码生成自动记录到 UIConfig 类中。
```csharp
public struct UIInfo
{
    public string LayerName { get; }                // 层级名
    public UIScheduleMode ScheduleMode { get; }     // 调度方式
    public string ResPath { get; }                  // 加载路径
}
```
&nbsp;

UI有三种调度方式：
+ Normal 调度：普通显示关闭
+ Queue  调度：队列头关闭后，才显示下一个队列元素。一般用于一次弹出多个 UI，逐一显示。
+ Stack  调度：UI Show 会进栈，将当前栈顶 UI 关闭。栈顶 UI 关闭时，将自己出栈，下一个栈顶 UI 显示。

&nbsp;

按下 ESC 键，默认关闭栈顶 UI，也可以自定义行为。
可以通过 UIManager.EscapeEvent，实现栈空时按下 ESC 键的行为。

## 快速开始
1. 点击 MenuItem：UniWork -> UIFramework -> 创建全部。<br>
这一步用于创建必要的资源配置，其中 UIRoot Prefab 和 UIRuntimeSetting 路径可变，但 UIEditorSetting 路径不可变。
2. 对 EditorSetting 和 RuntimeSetting 进行配置。
3. 创建 UIAgent.cs 脚本。
```csharp
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using UniWork.UIFramework.Runtime;
using UniWork.Utility.Runtime;

public class UIAgent : UIBaseAgent
{
    public override string RuntimeSettingLoadPath => "UIRuntimeSetting";

    public override void InitUIInfo()
    {
        foreach (var pair in UIConfig.InfoMap)
        {
            AddInfo(pair.Key, pair.Value);
        }
    }

    public override T Load<T>(string path)
    {
        return Resources.Load<T>(path);
    }

    public override async UniTask<T> LoadAsync<T>(string path)
    {
        return await Resources.LoadAsync<T>(path) as T;
    }

    public override void UnLoad(string path)
    {
        DLog.Info("UnLoad " + path);
    }
}
```
4. 将 `UIManager.Create(new UIAgent())` 添加到启动流程中。

## 代码生成功能
在 Prefab 上挂载 UICodeGenerator 脚本，点击对应按钮即可进行代码生成。<br>
注意：自动生成的 UIView 和 UICtrl 脚本的文件位置不能自行更改。<br>
控件的 Tag 如果是 AutoField，会根据 EditorSetting 中的配置自动绑定到 UIView 中。

## TODO
1. 文档完善
2. 打开关闭动画支持
3. 控件绑定方式更改 例如 txt_go|Name
4. 添加 UI SortingLayer
5. UIManager 改为非单例，非静态
6. UIManager 添加 Release 方法
7. UI Preview Scene 设定