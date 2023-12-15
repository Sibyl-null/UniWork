TODO: 表现样式实现，RedPointUI 编辑器扩展, 红点节点图

使用示例：
```
public static class RedPointDefine
{
    public static List<LeafNodeDefine> NodeDefines = new List<LeafNodeDefine>()
    {
        new LeafNodeDefine("Main|A|1", AOneFunc),
        new LeafNodeDefine("Main|A|2", ATwoFunc),
        new LeafNodeDefine("Main|B|1", BOneFunc),
        new LeafNodeDefine("Main|B|2", BTwoFunc),
    };

    public static bool AOneShow = true;
    private static bool AOneFunc()
    {
        return AOneShow;
    }
    
    public static bool ATwoShow = true;
    private static bool ATwoFunc()
    {
        return ATwoShow;
    }
    
    public static bool BOneShow = true;
    private static bool BOneFunc()
    {
        return BOneShow;
    }
    
    public static bool BTwoShow = true;
    private static bool BTwoFunc()
    {
        return BTwoShow;
    }
}
```

```
public class RedPointLauncher : MonoBehaviour
{
    private void Awake()
    {
        RedPointTree.Create(RedPointDefine.NodeDefines);

        GameObject prefab = Resources.Load<GameObject>("RedPointCanvas");
        Instantiate(prefab);
    }
}
```

```
public class RedPointCanvas : MonoBehaviour
{
    public RedPointUI AOne;
    public RedPointUI ATwo;
    public RedPointUI BOne;
    public RedPointUI BTwo;

    private void OnGUI()
    {
        if (GUILayout.Button("AOne"))
        {
            RedPointDefine.AOneShow = !RedPointDefine.AOneShow;
            AOne.Refresh();
        }
        
        if (GUILayout.Button("ATwo"))
        {
            RedPointDefine.ATwoShow = !RedPointDefine.ATwoShow;
            ATwo.Refresh();
        }
        
        if (GUILayout.Button("BOne"))
        {
            RedPointDefine.BOneShow = !RedPointDefine.BOneShow;
            BOne.Refresh();
        }
        
        if (GUILayout.Button("BTwo"))
        {
            RedPointDefine.BTwoShow = !RedPointDefine.BTwoShow;
            BTwo.Refresh();
        }
    }
}
```