# 引用池库使用
1. 使用引用池的类需要继承IReferenceRelease接口
2. ReferencePool 的 Spawn 和 Release 方法使用要一一对应。

```
public class Info : IReferenceRelease
{
    public void Release()
    {
        Debug.Log("Release");
    }
}

public class Launcher : MonoBehaviour
{
    private void Start()
    {
        ReferencePool.InitCapacity = 20;   //  可以设置内部栈的初始容量
        
        Info info = ReferencePool.Spawn<Info>();
        ReferencePool.Release(info);
    }
}
```