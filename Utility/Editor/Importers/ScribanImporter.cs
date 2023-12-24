using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace UniWork.Utility.Editor.Importers
{
    [ScriptedImporter(1, "scriban")]
    public class ScribanImporter : ScriptedImporter
    {
        /**
         * 使 .scriban 文件能被 unity 用 TextAsset 方式识别导入
         */
        public override void OnImportAsset(AssetImportContext ctx)
        {
            TextAsset textAsset = new TextAsset(File.ReadAllText(ctx.assetPath));
            ctx.AddObjectToAsset("text", textAsset);
            ctx.SetMainObject(textAsset);
        }
    }
}