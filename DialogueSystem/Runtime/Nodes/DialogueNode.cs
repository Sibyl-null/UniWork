using System.Collections.Generic;
using XNode;

namespace UniWork.DialogueSystem.Runtime.Nodes
{
    [System.Serializable]
    public class DialogueNode : BaseNode
    {
        [Input] public Empty input;
        [Output(dynamicPortList = true)] public List<Empty> outputs;

        public string talkName;
        public string content;
        
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}