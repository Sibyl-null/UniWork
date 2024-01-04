using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniWork.DialogueSystem.Editor
{
    public class DialogueGraphView : GraphView
    {
        public DialogueGraphView()
        {
            AddManipulators();
            AddStyle();

            AddGridBackground();
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
        }

        private void AddStyle()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("DialogueGraphView");
            styleSheets.Add(styleSheet);
        }
        
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
    }
}