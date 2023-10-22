namespace SFramework.BehaviourTree.Runtime.Attribute
{
    public enum ChildCapacity
    {
        None,       // 没有子节点
        Single,     // 一个子节点
        Multi       // 多个子节点
    }
    
    /// <summary>
    /// 子节点容量信息特性
    /// </summary>
    public class ChildCapacityInfoAttribute : System.Attribute
    {
        public ChildCapacity capacity;
    }
}