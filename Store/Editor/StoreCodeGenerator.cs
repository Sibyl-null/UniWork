using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UniWork.Store.Runtime;

namespace UniWork.Store.Editor
{
    public static class StoreCodeGenerator
    {
        public static string GenPath = "Assets/Scripts/AutoGen/Store";
        public static string Namespace = "AutoGen.Store";
        
        [MenuItem("UniWork/Store/StoreContainer 类生成")]
        public static void GenStoreContainer()
        {
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            Type[] types = assembly.GetTypes().Where(t =>
                t.GetInterfaces().Contains(typeof(IStoreData)) && !t.IsAbstract && !t.IsInterface).ToArray();

            HashSet<string> namespaceSet = CollectNamespaces(types);

            // ------------------------------------------------------
            
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine($"// Auto generate at {DateTime.Now.Date}");
            sb.AppendLine("// please do not modify this file");
            sb.AppendLine();
            
            foreach (string n in namespaceSet)
                sb.AppendLine($"using {n};");
            
            sb.AppendLine();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");
            sb.AppendLine("\t[MemoryPackable(GenerateType.VersionTolerant)]");
            sb.AppendLine("\tpublic partial class StoreContainer : StoreBaseContainer");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\t[MemoryPackOrder(0)] public Version Version;");

            for (int i = 0; i < types.Length; ++i)
                sb.AppendLine($"\t\t[MemoryPackOrder({i + 1})] public {types[i].Name} {types[i].Name};");

            sb.AppendLine();
            sb.AppendLine("\t\tpublic override void Init()");
            sb.AppendLine("\t\t{");
            
            foreach (Type type in types)
                sb.AppendLine($"\t\t\t{type.Name} = new {type.Name}();");

            sb.AppendLine("\t\t\tReset();");
            sb.AppendLine("\t\t}");

            sb.AppendLine();
            sb.AppendLine("\t\tpublic override void Reset()");
            sb.AppendLine("\t\t{");
            
            foreach (Type type in types)
                sb.AppendLine($"\t\t\t{type.Name}.Reset();");
            
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            
            // ------------------------------------------------------

            if (Directory.Exists(GenPath) == false)
                Directory.CreateDirectory(GenPath);
            
            File.WriteAllText(Path.Combine(GenPath, "StoreContainer.cs"), sb.ToString());
            AssetDatabase.Refresh();
        }

        private static HashSet<string> CollectNamespaces(Type[] types)
        {
            HashSet<string> namespaceSet = new HashSet<string>
            {
                "System",
                "MemoryPack",
                "UniWork.Store.Runtime"
            };

            foreach (Type type in types)
                namespaceSet.Add(type.Namespace);

            return namespaceSet;
        }
    }
}