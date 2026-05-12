using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Ray.FileIO
{
    public static class RayFileLoader
    {
        private static readonly string assetsPath = Application.streamingAssetsPath;

        public static T LoadJSON<T>(string path)
        {
            string filePath = Path.Combine(assetsPath, path);

            using StreamReader reader = new StreamReader(filePath, Encoding.UTF8);
            string json = reader.ReadToEnd();

            return JsonUtility.FromJson<T>(json);
        }

        public async static void LoadTextures(string folder, string[] targets, int offset, Texture2D[] results)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                results[offset + i] = await LoadTexture(folder + "/" + targets[i]);
            }
        }

        public async static Task<Texture2D> LoadTexture(string path)
        {
            string filePath = assetsPath + "/" + path;
            filePath = new Uri(filePath).AbsoluteUri;
            using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath, nonReadable: true);
            var operation = uwr.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
            return DownloadHandlerTexture.GetContent(uwr);
        }

        public static string[] LoadCSVAll(string path)
        {
            using StreamReader reader = new StreamReader(Path.Combine(assetsPath, path), Encoding.UTF8);
            return reader.ReadToEnd().Split(',');
        }

        public static string[] LoadCSVVertical(string path, int index)
        {
            using StreamReader reader = new StreamReader(Path.Combine(assetsPath, path), Encoding.UTF8);
            int num = int.Parse(reader.ReadLine());
            string[] result = new string[num];
            for (int i = 0; i < num; i++)
            {
                result[i] = reader.ReadLine().Split(',')[index];
            }
            return result;
        }
    }
}