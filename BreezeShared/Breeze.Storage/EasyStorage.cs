using System;
using System.Collections.Generic;
using System.Text;

namespace Breeze.Storage
{
    public class FileEntry
    {
        public string Folder { get; set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
    }

    public class Storage
    {
        public FileSystemEasyStorage FileSystemStorage = new FileSystemEasyStorage();
        public DatfileStorage DatfileStorage = new DatfileStorage();
        public UserEasyStorage UserStorage = new UserEasyStorage();
    }
}
