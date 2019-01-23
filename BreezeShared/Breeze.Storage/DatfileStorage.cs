using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Breeze.Helpers;
using Breeze.Shared;

namespace Breeze.Storage
{
    public class DatfileStorage : IEasyStorage
    {
        private List<FileEntry> tableOfContents = new List<FileEntry>();

        public List<FileEntry> TabletOfContents
        {
            get { return tableOfContents; }
            set { tableOfContents = value; }
        }
        public byte[] DataStorage;

        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetFiles(string path)
        {
            if (path.EndsWith("\\"))
            {
                path = path.Substring(0, path.Length - 1);
            }

            
            var result = TabletOfContents.Where(t => t.Folder.ToLower() == path.ToLower()).ToList();

            return result.Select(t => t.FileName);
        }

        public IEnumerable<string> GetFiles(string path, string pattern)
        {
            if (path.EndsWith("\\"))
            {
                path = path.Substring(0, path.Length - 1);
            }

            var parts = pattern.ToLower().Split('*');
            var result = TabletOfContents.Where(t => t.Folder.ToLower() == path.ToLower()).ToList();

            result = result.Where(t => t.FileName.ToLower().StartsWith(parts[0].ToLower())).ToList();
            result = result.Where(t => t.FileName.ToLower().EndsWith(parts[1].ToLower())).ToList();
            return result.Select(t => t.FileName);
        }

        public IEnumerable<string> GetFolders(string path)
        {
            if (path.EndsWith("\\"))
            {
                path = path.Substring(0, path.Length - 1);
            }


            var result = TabletOfContents.Where(t => t.Folder.ToLower() == path.ToLower()).ToList();

            return result.Select(t => t.FileName); ;
        }

        public IEnumerable<string> GetFolders(string path, string pattern)
        {
            if (path.EndsWith("\\"))
            {
                path = path.Substring(0, path.Length - 1);
            }

            var parts = pattern.ToLower().Split('*');
            var result = TabletOfContents.Where(t => t.Folder.ToLower() == path.ToLower()).ToList();

            result = result.Where(t => t.FileName.ToLower().StartsWith(parts[0].ToLower())).ToList();
            result = result.Where(t => t.FileName.ToLower().EndsWith(parts[1].ToLower())).ToList();
            return result.Select(t => t.FileName);
        }

        public string ReadText(string path)
        {
            if (path.StartsWith("\\")) path = path.Substring(1);
            FileEntry entry = TabletOfContents.First(t => t.FullPath.ToLower() == path.ToLower());
            byte[] result = new byte[entry.Length];

            Buffer.BlockCopy(DataStorage, entry.Start, result, 0, entry.Length);

            using (MemoryStream memoryStream = new MemoryStream(result))
            {
                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public byte[] ReadBytes(string path)
        {
            Debug.WriteLine("Read Bytes: " + path);
            if (path.StartsWith("\\")) path = path.Substring(1);
            FileEntry entry = TabletOfContents.First(t => t.FullPath.ToLower() == path.ToLower());
            byte[] result = new byte[entry.Length];

            Buffer.BlockCopy(DataStorage, entry.Start, result, 0, entry.Length);

            return result;
        }

        public Stream GetStream(string path)
        {
            
            if (path.StartsWith("\\")) path = path.Substring(1);
            FileEntry entry = TabletOfContents.First(t => t.FullPath.ToLower() == path.ToLower());
            byte[] result = new byte[entry.Length];

            Buffer.BlockCopy(DataStorage, entry.Start, result, 0, entry.Length);

            return new MemoryStream(result);
        }

        public T ReadJson<T>(string path)
        {
            throw new NotImplementedException();
        }

        public void CreateFolder(string path)
        {
            throw new NotImplementedException();
        }

        public Stream CreateStream(string path)
        {
            throw new NotImplementedException();
        }

        public void WriteBytes(string path, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void WriteText(string path, string text)
        {
            throw new NotImplementedException();
        }

        public void WriteJson<T>(string path, T data)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string path)
        {
            return TabletOfContents.Any(t => t.FullPath.ToLower() == path.ToLower());
        }

        public bool FolderExists(string path)
        {
            return TabletOfContents.Any(t => t.FullPath.ToLower() == path.ToLower());
        }

        public void CreateFoldersForCompletePath(string path)
        {
            throw new NotImplementedException();
        }
    }
}