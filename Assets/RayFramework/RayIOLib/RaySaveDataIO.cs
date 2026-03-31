using System.IO;
using System.Text;
using UnityEngine;

namespace Ray.FileIO
{
    public static class RaySaveDataIO
    {
        private static readonly string originalPath = Path.Combine(Application.streamingAssetsPath, "SaveDataOriginal");
        private static readonly string savedataPath = Path.Combine(Application.streamingAssetsPath, "SaveData");

        public static T LoadSaveData<T>(string path) where T : class
        {
            if (!File.Exists(Path.Combine(savedataPath, path)))
            {
                SaveJson<T>(path, LoadJSON<T>(originalPath, path));
            }
            return LoadJSON<T>(savedataPath, path);
        }

        private static T LoadJSON<T>(string parentPath, string path) where T : class 
        {
            string filePath = Path.Combine(parentPath, path);

            using StreamReader reader = new StreamReader(filePath, Encoding.UTF8);
            string json = reader.ReadToEnd();

            return JsonUtility.FromJson<T>(json);
        }

        public static void SaveJson<T>(string path, T obj) where T : class
        {
            if(!Directory.Exists(savedataPath))
            {
                Directory.CreateDirectory(savedataPath);
            }
            string json = JsonUtility.ToJson(obj, true);
            File.WriteAllText(Path.Combine(savedataPath, path), json);
        }

        public static void Initialize<T>(string path) where T : class
        {
            SaveJson<T>(path, LoadJSON<T>(originalPath, path));
        }
    }
}
