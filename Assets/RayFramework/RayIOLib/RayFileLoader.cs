using System.IO;
using System.Text;
using UnityEngine;

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

        public static Sprite LoadSprite(string path)
        {
            string filePath = Path.Combine(assetsPath, path);
            byte[] rawData = System.IO.File.ReadAllBytes(filePath);
            Texture2D texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(rawData);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
            return sprite;
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