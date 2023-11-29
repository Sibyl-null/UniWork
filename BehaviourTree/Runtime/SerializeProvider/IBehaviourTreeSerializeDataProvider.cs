namespace UniWork.BehaviourTree.Runtime.SerializeProvider
{
    public interface IBehaviourTreeSerializeDataProvider
    {
        void Serialize(BehaviourTree tree);
        BehaviourTree Deserialize();
    }
}