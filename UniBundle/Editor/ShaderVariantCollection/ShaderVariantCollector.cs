using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UniWork.Utility.Runtime;

namespace UniWork.UniBundle.Editor.ShaderVariantCollection
{
    public static class ShaderVariantCollector
    {        
        public static void Run(List<string> materials, string savePath)
        {
            PrepareCollection(savePath);
            ProcessCollection(materials, savePath);
        }

        private static void PrepareCollection(string savePath)
        {
            string directory = Path.GetDirectoryName(savePath);
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            if (File.Exists(savePath))
                AssetDatabase.DeleteAsset(savePath);
            
            // 聚焦到 GameView 窗口
            System.Type type = Assembly.Load("UnityEditor").GetType("UnityEditor.GameView");
            EditorWindow.GetWindow(type, false, "GameView", true);
            
            // 创建一个带相机和光源的临时场景
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        }
        
        private static void ProcessCollection(List<string> materials, string savePath)
        {
            ShaderVariantHelper.ClearCurrentVariantRecord();

            Camera camera = Camera.main;
            if (camera == null)
                throw new BuildFailedException("[ShaderVariantCollector] Not find main camera");
            
            // 设置主相机
            float aspect = camera.aspect;
            int totalMaterials = materials.Count;
            float height = Mathf.Sqrt(totalMaterials / aspect) + 1;
            float width = Mathf.Sqrt(totalMaterials / aspect) * aspect + 1;
            float halfHeight = Mathf.CeilToInt(height / 2f);
            float halfWidth = Mathf.CeilToInt(width / 2f);
            camera.orthographic = true;
            camera.orthographicSize = halfHeight;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            
            // 创建测试球体
            int xMax = (int)(width - 1);
            int x = 0, y = 0;
            int progressValue = 0;
            for (int i = 0; i < materials.Count; i++)
            {
                string material = materials[i];
                Vector3 position = new Vector3(x - halfWidth + 1f, y - halfHeight + 1f, 0f);
                GameObject go = CreateSphere(material, position, i);
                if (go == null)
                    continue;
                
                if (x == xMax)
                {
                    x = 0;
                    y++;
                }
                else
                {
                    x++;
                }

                ++progressValue;
                EditorUtility.DisplayProgressBar("变体收集进度", $"照射所有材质球 : {progressValue}/{materials.Count}",
                    progressValue / (float)materials.Count);
            }
            EditorUtility.ClearProgressBar();
            
            ShaderVariantHelper.SaveCurrentVariantRecord(savePath);
            AssetDatabase.Refresh();
            DLog.Info("[ShaderVariantCollector] 收集完毕");
        }

        private static GameObject CreateSphere(string materialPath, Vector3 position, int index)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            Shader shader = material.shader;
            if (shader == null)
                return null;

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.GetComponent<Renderer>().sharedMaterial = material;
            go.transform.position = position;
            go.name = $"Sphere_{index} | {material.name}";
            return go;
        }
    }
}