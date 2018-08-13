using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Breeze.Helpers;
using Microsoft.Xna.Framework;

#if WINDOWS_UAP
using Windows.Storage;
#endif

namespace Breeze.Storage.Helpers
{
    public static class FileHelpers
    {
        //public static DirCacheProvider CacheProvider = new DirCacheProvider();

        public static bool FolderExists(this string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        public static string SanitizeFileName(this string filename)
        {
            if (filename == null)
            {
                return null;
            }

            string result = "";

            string bannedChars = ":\\/\"';.";

            for (int i = 0; i < filename.Length; i++)
            {
                if (!bannedChars.Contains(filename.Substring(i, 1)))
                {
                    result = result + filename.Substring(i, 1);
                }
            }

            return result;
        }

        //public static async Task<StorageFolder> GetFolderAsyncSafe(this StorageFolder folder, string folderName)
        //{
        //    if (!folderName.FolderExists())
        //    {
        //        try
        //        {
        //            await folder.CreateFolderAsync(folderName);
        //        }
        //        catch
        //        {
        //        }
        //    }

        //    return await folder.GetFolderAsync(folderName);

        //}

        public static async Task<byte[]> ToByteArray(this string file)
        {
            byte[] result;
            using (Stream stream = await file.ToStream())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {

                    stream.CopyTo(memoryStream);
                    result = memoryStream.ToArray();
                }
            }

            return result;
        }

        public static string GetFile(this string path, string file)
        {
            return GetFolder(path, file);
        }
        public static string GetFolder(this string path, string folder)
        {
            string result = path;
            if (!path.EndsWith("\\") && !folder.StartsWith("\\"))
            {
                result = result + "\\";
            }

            if (path.EndsWith("\\") && folder.StartsWith("\\"))
            {
                result = result + folder.Substring(1);
            }
            else
            {
                result = result + folder;
            }

            return result;
        }

        public static async Task<T> DeserializeXMLFromFile<T>(this string path)
        {
#if WINDOWS_UAP
            StorageFolder tmp = null;
            string part = "";

            if (path.StartsWith(FileLocation.InstalledLocation))
            {
                part = path.Substring(FileLocation.InstalledLocation.Length);
                tmp = Windows.ApplicationModel.Package.Current.InstalledLocation;
            }

            //if (path.StartsWith(FileLocation.LocalFolder))
            //{
            //    part = path.Substring(FileLocation.LocalFolder.Length);
            //    tmp = ApplicationData.Current.LocalFolder;
            //}

            if (tmp == null)
            {
                part = Solids.Instance.ContentPathRelative.GetFile(path);
                tmp = Windows.ApplicationModel.Package.Current.InstalledLocation;
            }

            XmlSerializer deserializer = new XmlSerializer(typeof(T));

            using (var fs = await tmp.OpenStreamForReadAsync(part))
            {

                using (TextReader textReader = new StreamReader(fs))
                {
                    T file = (T)deserializer.Deserialize(textReader);
                    return file;
                }
            }
#else
            
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
    using(FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read)){
     using (TextReader textReader = new StreamReader(fs))
                {
                    T file = (T) deserializer.Deserialize(textReader);
                    return file;
                }
    }
#endif

        }


#if ANDROID
        public static Stream FromAsset(this string path)
        {
            path = path.Replace("\\\\", "\\");
            if (path.StartsWith("\\"))
            {
                path = path.Substring(1);
            }

            
         //   Debug.WriteLine("Loading " + path);
            
            var tst = Game.Activity.Assets.Open(path);
            return tst;
        }
#endif



        public static async Task<Stream> ToStream(this string path)
        {
            path = path.Replace("\\\\", "\\");
            Debug.WriteLine("Loading " + path);
#if WINDOWS_UAP

            if (path.StartsWith(FileLocation.InstalledLocation))
            {
                string part = path.Substring(FileLocation.InstalledLocation.Length);
                StorageFolder tmp = Windows.ApplicationModel.Package.Current.InstalledLocation;
                return await tmp.OpenStreamForReadAsync(part);
            }

            //if (path.StartsWith(FileLocation.LocalFolder))
            //{
            //    string part = path.Substring(FileLocation.LocalFolder.Length);
            //    StorageFolder tmp = ApplicationData.Current.LocalFolder;
            //    return await tmp.OpenStreamForReadAsync(part);
            //}

            StorageFile file = await StorageFile.GetFileFromPathAsync(path);

            Stream stream = await file.OpenStreamForReadAsync();
            return stream;
#endif

#if ANDROID
            if (path.StartsWith("\\")) path = path.Substring(1);
            var tst = Game.Activity.Assets.Open(path);
            return tst;

#endif

            if (path.StartsWith("\\")) path = path.Substring(1);
            FileStream st = File.Open(path, FileMode.Open, FileAccess.Read);
            return st;
        }


        public static void CreateFolder(this string path)
        {
            Directory.CreateDirectory(path);
        }


        public static void WriteBytes(this string file, byte[] bytes)
        {
            using (var st = new FileStream(file, FileMode.Create))
            {
                st.Write(bytes, 0, bytes.Length);
            }
        }

        public static void CreateFolder(this string path, string name)
        {
            Directory.CreateDirectory(path.GetFile(name));
        }
    }
}
