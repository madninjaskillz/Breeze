using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Breeze.Storage
{
    public class UserEasyStorage : IEasyStorage
    {
        IsolatedStorageFile dataFile = IsolatedStorageFile.GetUserStoreForApplication();
        public async Task Initialize()
        {
            
        }

        public IEnumerable<string> GetFiles(string path)
        {
            if (!path.Contains("*"))
            {
                if (path.Length > 0 && !path.EndsWith("\\"))
                {
                    path = path + "\\";
                }

                path = path + "*";
            }

            var result = dataFile.GetFileNames(path);
            return result.ToList();
        }

        public IEnumerable<string> GetFiles(string path, string pattern)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetFolders(string path)
        {
            if (!path.Contains("*"))
            {
                if (path.Length > 0 && !path.EndsWith("\\"))
                {
                    path = path + "\\";
                }

                path = path + "*";
            }

            return dataFile.GetDirectoryNames(path);
        }

        public IEnumerable<string> GetFolders(string path, string pattern)
        {
            throw new NotImplementedException();
        }

        public string ReadText(string path)
        {
            using (var st = dataFile.OpenFile(path, FileMode.Open))
            {
                using (var r = new StreamReader(st))
                {
                    return r.ReadToEnd();
                }
            }
        }

        public byte[] ReadBytes(string path)
        {
            using (var st = dataFile.OpenFile(path, FileMode.Open))
            {
                byte[] result = new byte[st.Length];
                st.Read(result, 0, (int)st.Length);

                return result;
            }
        }

        public Stream GetStream(string path)
        {
            return dataFile.OpenFile(path, FileMode.Open);
        }

        public T ReadJson<T>(string path)
        {
            string txt = ReadText(path);
            try
            {
                return (T)JsonConvert.DeserializeObject(txt, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public void CreateFolder(string path)
        {
            dataFile.CreateDirectory(path);
        }

        public Stream CreateStream(string path)
        {
            CreateFoldersForCompletePath(path);
            return dataFile.CreateFile(path);
        }

        public void WriteBytes(string path, byte[] data)
        {
            using (var st = CreateStream(path))
            {
                st.Write(data, 0, data.Length);
            }
        }

        public void WriteText(string path, string text)
        {
            CreateFoldersForCompletePath(path);

            using (var st = dataFile.OpenFile(path, FileMode.Create))
            {
                using (var sw = new StreamWriter(st))
                {
                    sw.Write(text);
                }

            }
        }

        public void WriteJson<T>(string path, T data)
        {
            string json = JsonConvert.SerializeObject(data);
            WriteText(path, json);
        }

        public bool FileExists(string path)
        {
            return dataFile.FileExists(path);
        }

        public bool FolderExists(string path)
        {
            return dataFile.DirectoryExists(path);
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

                    if (!dataFile.DirectoryExists(fullPath))
                    {
                        dataFile.CreateDirectory(fullPath);
                    }
                }

            }
        }
    }
}