# Store 存储库
## 基本介绍
使用 MemoryPack 库进行序列化和反序列化。根据自身情况选择是否使用完全版本兼容模式。

## 使用示例
1. 需要存储的数据结构继承 IStoreData 接口。
```csharp
[MemoryPackable(GenerateType.VersionTolerant)]
public partial class AStoreData : IStoreData
{
    [MemoryPackOrder(0)]
    public string Str;

    public void Reset()
    {
        Str = "";
    }
}

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class BStoreData : IStoreData
{
    [MemoryPackOrder(0)]
    public int Num;

    public void Reset()
    {
        Num = 0;
    }
}
```

2. 点击 MenuItem（ UniWork/Store/StoreContainer 类生成） 自动生成 StoreContainer 类。

3. 调用 StoreRoot 相关 Api 进行使用：
```csharp
public class Test : MonoBehaviour
{
    private void Awake()
    {
        StoreRoot.Create(typeof(StoreContainer));            
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button)
        {
            fontSize = 30
        };
        
        if (GUILayout.Button("修改数据", style))
        {
            StoreContainer container = StoreRoot.BaseContainer as StoreContainer;
            container.AStoreData.Str = "abc";
            container.BStoreData.Num = 50;
        }
        
        if (GUILayout.Button("打印数据", style))
        {
            StoreContainer container = StoreRoot.BaseContainer as StoreContainer;
            DLog.Info(container.AStoreData.Str);
            DLog.Info(container.BStoreData.Num.ToString());
        }
        
        if (GUILayout.Button("保存数据", style))
        {
            StoreRoot.Save();
        }
    }
}
```