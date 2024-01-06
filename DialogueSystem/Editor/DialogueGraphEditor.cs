using System;
using UniWork.DialogueSystem.Runtime;
using UniWork.DialogueSystem.Runtime.Nodes;
using XNodeEditor;

namespace UniWork.DialogueSystem.Editor
{
    [CustomNodeGraphEditor(typeof(DialogueGraph))]
    public class DialogueGraphEditor : NodeGraphEditor
    {
        public override string GetNodeMenuName(Type type)
        {
            if (typeof(BaseNode).IsAssignableFrom(type))
            {
                return type.Name;
            }

            return null;
        }
    }
}
