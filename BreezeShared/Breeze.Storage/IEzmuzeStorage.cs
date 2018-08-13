using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.Storage
{
    public interface IEasyStorage
    {
        Task Initialize();
        IEnumerable<string> GetFiles(string path);
        IEnumerable<string> GetFiles(string path, string pattern);
        IEnumerable<string> GetFolders(string path);
        IEnumerable<string> GetFolders(string path, string pattern);

        string ReadText(string path);
        byte[] ReadBytes(string path);
        Stream GetStream(string path);
        T ReadJson<T>(string path);

        void CreateFolder(string path);
        Stream CreateStream(string path);

        void WriteBytes(string path, byte[] data);
        void WriteText(string path, string text);
        void WriteJson<T>(string path, T data);

        bool FileExists(string path);
        bool FolderExists(string path);
        void CreateFoldersForCompletePath(string path);
    }
}
