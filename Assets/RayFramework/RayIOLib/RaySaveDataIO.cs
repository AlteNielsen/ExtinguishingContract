using System.IO;
using System.Text;
using UnityEngine;

namespace Ray.FileIO
{
    public static class RaySaveDataIO
    {
        private static readonly string originalPath = Path.Combine(Application.streamingAssetsPath, "SaveDataOriginal");
        private static readonly string savedataPath = Path.Combine(Application.persistentDataPath, "SaveData");

        public static T LoadSaveData<T>(string path) where T : class
        {
            if (!File.Exists(Path.Combine(savedataPath, path)))
            {
                SaveJson<T>(path, LoadJSON<T>(Path.Combine(originalPath, path), false));
            }
            T data = LoadJSON<T>(Path.Combine(savedataPath, path), false);
            if(data == null)
            {
                data = LoadJSON<T>(Path.Combine(savedataPath, path), true);
            }
            return data;
        }

        private static T LoadJSON<T>(string filePath, bool isBak) where T : class 
        {
            string json = null;
            if (isBak && File.Exists(filePath + ".bak"))
            {
                using StreamReader reader = new StreamReader(filePath + ".bak", Encoding.UTF8);
                json = reader.ReadToEnd();
            }
            else
            {
                using StreamReader reader = new StreamReader(filePath, Encoding.UTF8);
                json = reader.ReadToEnd();
            }

            try
            {
                T data = JsonUtility.FromJson<T>(json);
                return data;
            }
            catch
            {
                return null;
            }
        }

        public static void SaveJson<T>(string path, T obj) where T : class
        {
            if(!Directory.Exists(savedataPath))
            {
                Directory.CreateDirectory(savedataPath);
            }
            string json = JsonUtility.ToJson(obj, true);

            string finalPath = Path.Combine(savedataPath, path);

            if(File.Exists(finalPath))
            {
                File.Copy(finalPath, finalPath + ".bak", overwrite: true);
            }

            File.WriteAllText(finalPath + ".tmp", json);
            if(File.Exists(finalPath))
            {
                File.Delete(finalPath);
            }
            File.Move(finalPath + ".tmp", finalPath);
        }

        public static void Initialize<T>(string path) where T : class
        {
            SaveJson<T>(path, LoadJSON<T>(Path.Combine(originalPath, path), false));
        }
    }
}
