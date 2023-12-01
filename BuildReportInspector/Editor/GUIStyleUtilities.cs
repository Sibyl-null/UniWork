using UnityEditor;
using UnityEngine;

namespace UniWork.BuildReportInspector.Editor
{
    public static class GUIStyleUtilities
    {
        public static GUIStyle SizeStyle { get; } = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleRight
        };

        public static GUIStyle OddStyle { get; } = new GUIStyle(GUIStyle.none)
        {
            normal = { background = MakeColorTexture(new Color(0.5f, 0.5f, 0.5f, 0.1f)) }
        };

        public static GUIStyle EvenStyle { get; } = new GUIStyle(GUIStyle.none)
        {
            normal = {background = MakeColorTexture(new Color(0.5f, 0.5f, 0.5f, 0.0f))}
        };

        public static GUIStyle DataFileStyle { get; } = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold
        };

        private static Texture2D MakeColorTexture(Color col)
        {
            var pix = new Color[1];
            pix[0] = col;

            var result = new Texture2D(1, 1);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}