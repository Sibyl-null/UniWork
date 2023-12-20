using UnityEditor;
using UniWork.Utility.Runtime.MethodUtility;

namespace UniWork.UniBundle.Editor.ShaderVariantCollection
{
    internal static class ShaderVariantHelper
    {
        internal static void ClearCurrentVariantRecord()
        {
            ReflectUtility.InvokeNonPublicStaticMethod(typeof(ShaderUtil), "ClearCurrentShaderVariantCollection");
        }

        internal static void SaveCurrentVariantRecord(string savePath)
        {
            ReflectUtility.InvokeNonPublicStaticMethod(typeof(ShaderUtil), "SaveCurrentShaderVariantCollection",
                savePath);
        }
    }
}