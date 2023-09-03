using System.Collections.Generic;
using System.IO;

namespace SFramework.Utility.Runtime
{
    public static class IoUtility
    {
        public static bool IsFile(string path)
        {
            return File.Exists(path);
        }

        public static bool IsDirectory(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// 获取所有子文件夹，不包含中间文件夹和自己
        /// </summary>
        public static List<string> GetLeafDirectories(string path)
        {
            List<string> leafDirPathList = new List<string>();
            if (Directory.Exists(path) == false)
                return leafDirPathList;

            Queue<string> pathQue = new Queue<string>();
            pathQue.Enqueue(path);
            
            while (pathQue.Count > 0)
            {
                string dirPath = pathQue.Dequeue();
                string[] subDirPaths = Directory.GetDirectories(dirPath);

                if (subDirPaths.Length > 0)
                {
                    foreach (string subDirPath in subDirPaths)
                        pathQue.Enqueue(subDirPath);                        
                }
                else
                    leafDirPathList.Add(dirPath);
            }

            return leafDirPathList;
        }

        /// <summary>
        /// 创建一个新的文件夹，如果原本有内容则清空
        /// </summary>
        public static void MakeNewDirectory(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path,true);
            
            Directory.CreateDirectory(path);
        }
    }
}