using UnityEngine;

namespace SFramework.UIFramework.Runtime
{
    [CreateAssetMenu(menuName = "SFramework/UI/UIRuntimeSetting", fileName = "UIRuntimeSetting")]
    public class UIRuntimeSetting : ScriptableObject
    {
        public float width = 1020;
        public float height = 720;
        public float match = 1;
    }
}