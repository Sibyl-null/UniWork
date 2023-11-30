using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace UniWork.Utility.Editor.PrefabHistoryTab
{
    public class PrefabHistoryRecord
    {
        private const string FilePath = "Library/PrefabHistoryRecord.json";
        
        public readonly List<string> PrefabPaths = new List<string>();

        public void AddRecord(string path)
        {
            if (HasRecord(path))
                return;
            
            PrefabPaths.Add(path);
            Save();
        }
        
        public void RemoveRecord(string path)
        {
            if (HasRecord(path) == false)
                return;

            PrefabPaths.Remove(path);
            Save();
        }

        public bool HasRecord(string path)
        {
            return PrefabPaths.Contains(path);
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(FilePath, json);
        }
        
        public static PrefabHistoryRecord LoadRecord()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<PrefabHistoryRecord>(json);
            }
            
            return new PrefabHistoryRecord();
        }
    }
}