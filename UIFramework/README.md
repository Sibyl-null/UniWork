# UIFramework 使用
## 基本介绍
一个UI面板用一个Canvas进行管理，UIBaseView的子类View挂载到Prefab上，用于关联需要的组件。 
UIBaseCtrl的子类持有对应的View对象，用于实现交互逻辑。

&nbsp;

每个UI面板都有一个UIInfo信息来描述自己。
```
public class UIInfo
{
    public UIEnumBaseType UIEnumBaseType;    // 标识
    public UIEnumBaseLayer UIEnumBaseLayer;  // 层级
    public Type ViewType;                    // View类的Type
    public Type CtrlType;                    // Ctrl类的Type
    public UIScheduleMode ScheduleMode;      // 调度模式
    public string ResPath;                   // 加载路径
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

## 使用示例
1. 继承UIEnumBaseType，实现自己的UI标识枚举类
```
public class UIEnumType : UIEnumBaseType
{
    private UIEnumType(int key, string value) : base(key, value)
    {
    }
    
    // UI标识，当作枚举使用
    public static readonly UIEnumType UITestOne = new UIEnumType(0, "UITestOne");
    public static readonly UIEnumType UITestTwo = new UIEnumType(1, "UITestTwo");
}
```
2. 继承UIEnumBaseLayer，实现自己的UI层级枚举类
```
public class UIEnumLayer : UIEnumBaseLayer
{
    private UIEnumLayer(int key, string value) : base(key, value)
    {
    }

    // 定义的int数值不要相同，也不要差距太小。 表示 Canvas 的 Order In Layer 值
    public static readonly UIEnumLayer Bottom = new UIEnumLayer(0, "Normal");
    public static readonly UIEnumLayer Middle = new UIEnumLayer(1000, "Middle");
    public static readonly UIEnumLayer Top = new UIEnumLayer(2000, "Top");
}
```
3. 继承UIManagerBaseAgent，实现自己的UIManager代理类
```
public class UIMgrAgent : UIManagerBaseAgent
{
    // 加载UIRoot和UIRuntimeSetting使用的路径
    public override string UIRootLoadPath => "UIRoot";
    public override string RuntimeSettingLoadPath => "UIRuntimeSetting";

    public override ReadOnlyCollection<UIEnumBaseLayer> GetAllLayers()
    {
        // 返回自定义的UILayer枚举类对象列表
        return UIEnumLayer.GetAllEnumTypes();
    }
    
    public override T Load<T>(string path)
    {
        // 返回加载资源的方式，Resources / AssetBundle ...
        return Resources.Load<T>(path);
    }

    // 在这里添加所有UIInfo信息
    public override void InitUIInfo()
    {
        AddInfo(UIEnumType.UITestOne, new UIInfo
        {
            UIEnumBaseType = UIEnumType.UITestOne,
            UIEnumBaseLayer = UIEnumLayer.Middle,
            ViewType = typeof(UITestOneView),
            CtrlType = typeof(UITestOneCtrl),
            ScheduleMode = UIScheduleMode.Stack,
            ResPath = "UITestOne",
        });
    }
}
```
4. 通过MenuItem功能，创建UIRoot的预制体。自行调整存放位置匹配加载路径。
5. 通过MenuItem功能，创建UIRuntimeSetting。自行调整存放位置匹配加载路径。

## Editor功能
MenuItem：SFramework -> UIFramework
其中自动生成 View Ctrl 代码的 EditorWindow 需要 Odin 插件才会生效。