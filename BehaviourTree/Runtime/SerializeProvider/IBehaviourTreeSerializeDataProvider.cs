namespace SFramework.BehaviourTree.Runtime.SerializeProvider
{
    public interface IBehaviourTreeSerializeDataProvider
    {
        void Serialize(BehaviourTree tree);
        BehaviourTree Deserialize();
    }
}