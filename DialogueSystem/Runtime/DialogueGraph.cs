using UnityEngine;
using UniWork.DialogueSystem.Runtime.Nodes;
using XNode;

namespace UniWork.DialogueSystem.Runtime
{
    [CreateAssetMenu(menuName = "UniWork/DialogueGraph", fileName = "NewDialogueGraph")]
    public class DialogueGraph : NodeGraph
    {
        [field:SerializeField]
        public RootNode RootNode { get; set; }
    }
}