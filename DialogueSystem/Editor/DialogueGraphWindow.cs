using UnityEditor;
using UnityEngine.UIElements;

namespace UniWork.DialogueSystem.Editor
{
    public class DialogueGraphWindow : EditorWindow
    {
        [MenuItem("UniWork/DialogueSystem/Open GraphWindow")]
        private static void OpenWindow()
        {
            GetWindow<DialogueGraphWindow>("DialogueGraph");
        }

        private void OnEnable()
        {
            DialogueGraphView view = new DialogueGraphView();
            view.StretchToParentSize();
            rootVisualElement.Add(view);
        }
    }
}
