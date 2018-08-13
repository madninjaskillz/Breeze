using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PakBuilder
{
    class Program
    {
        static string rootpath = "D:\\Projects\\ezmuzepro\\Ezmuze.Shared\\Ezmuze.Content\\Content\\";
        static void Main(string[] args)
        {

            rootpath = args[0];
           // rootpath = "O:\\ezmuzepro\\Ezmuze.Shared\\Ezmuze.Content\\Content\\";
            List<string> fullPaths = new List<string>();

            fullPaths = GetFolder(rootpath);
          
            List<FileEntry> toc = new List<FileEntry>();
            using (MemoryStream ms = new MemoryStream())
            {
                foreach (string fullPath in fullPaths)
                {
                    FileEntry entry = new FileEntry();

                    entry.FileName = fullPath.Split('\\').Last();
                    entry.Folder = fullPath.Substring(0, fullPath.Length - entry.FileName.Length);
                    if (entry.Folder.EndsWith("\\"))
                    {
                        entry.Folder = entry.Folder.Substring(0, entry.Folder.Length - 1);
                    }

                    entry.FullPath = fullPath;
                    entry.Start = (int) ms.Position;

                    using (var stream = new FileStream(rootpath+fullPath, FileMode.Open))
                    {
                        entry.Length = (int) stream.Length;
                        byte[] tmp = new byte[(int) stream.Length];
                        stream.Read(tmp, 0, tmp.Length);
                        ms.Write(tmp, 0, tmp.Length);
                    }

                    Debug.WriteLine(entry);

                    toc.Add(entry);
                }

                string tocFile = rootpath.Substring(0, rootpath.Length - 1) + ".toc";
                string pakFile = rootpath.Substring(0, rootpath.Length - 1) + ".pak";
                using (var st = new FileStream(tocFile, FileMode.Create))
                {
                    string json = JsonConvert.SerializeObject(toc);
                    using (var sw = new StreamWriter(st))
                    {
                        sw.Write(json);
                    }
                }

                byte[] data = ms.ToArray();
                using (var st = new FileStream(pakFile, FileMode.Create))
                {
                    st.Write(data, 0, data.Length);
                }
            }
        }

        public class FileEntry
        {
            public string Folder { get; set; }
            public string FileName { get; set; }
            public string FullPath { get; set; }
            public int Start { get; set; }
            public int Length { get; set; }
        }

        static List<string> GetFolder(string path)
        {
            List<string> result = new List<string>();
            var files = Directory.GetFiles(path);

            result.AddRange(files.Select(x=>x.Replace(rootpath, "")));
            
            var dirs = Directory.GetDirectories(path);

            result.AddRange(dirs.SelectMany(GetFolder));

            return result;
        }
    }
}
