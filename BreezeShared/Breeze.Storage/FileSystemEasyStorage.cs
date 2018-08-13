using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breeze.Helpers;
using Breeze.Storage.Helpers;
using Newtonsoft.Json;


namespace Breeze.Storage
{
    public class FileSystemEasyStorage : IEasyStorage
    {
        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public IEnumerable<string> GetFiles(string path, string pattern)
        {
            return Directory.GetFiles(path.EnsureEndsWith("\\") + pattern);
        }

        public IEnumerable<string> GetFolders(string path)
        {
            return Directory.GetDirectories(path);
        }

        public IEnumerable<string> GetFolders(string path, string pattern)
        {
            return Directory.GetDirectories(path.EnsureEndsWith("\\" + pattern));
        }

        public string ReadText(string path)
        {
            return File.ReadAllText(path);
        }

        public byte[] ReadBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public Stream GetStream(string path)
        {
            return File.Open(path, FileMode.Open);
        }

        public T ReadJson<T>(string path)
        {
            string txt = ReadText(path);
            return JsonConvert.DeserializeObject<T>(txt);
        }

        public void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        public Stream CreateStream(string path)
        {
            return File.Create(path);
        }

        public void WriteBytes(string path, byte[] data)
        {
            File.WriteAllBytes(path, data);
        }

        public void WriteText(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        public void WriteJson<T>(string path, T data)
        {
            string json = JsonConvert.SerializeObject(data);
            WriteText(path,json);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateFoldersForCompletePath(string path)
        {
            string[] parts = path.Split('\\');
            if (parts.Length < 2)
            {
                return;
            }

            for (int i = 1; i < parts.Length; i++)
            {
                string fullPath = "";
                for (int x = 0; x < i; x++)
                {
                    fullPath = fullPath + parts[x] + "\\";

                }

                if (fullPath.Length > 0)
                {
                    fullPath = fullPath.Substring(0, fullPath.Length - 1);

                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                }

            }
        }
    }
}
