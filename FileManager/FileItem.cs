using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class FileItem
    {
        public string Name { get; }
        public string Extension { get; }
        public int Index { get; }
        public string Size { get; set; }
        public string FullPath { get; set; }
        public DateTime LastReadTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public bool IsReadOnly { get; set; }

        public FileItem(int index, string name, string extension = "dir")
        {
            Index = index;
            Name = name;
            Extension = extension;
        }
    }
}
