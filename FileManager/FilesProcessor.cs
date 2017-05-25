using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class FilesProcessor
    {
        private FileManager fileManager;

        public bool IsCutted { get; set; }

        public FilesProcessor(FileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public void FileRename(string oldName, string newName)
        {
            File.Move(oldName, newName);
        }

        public void DirectoryRename(string oldName, string newName)
        {
            try
            {
                Directory.Move(oldName, newName);
            }
            catch (Exception e)
            {
                fileManager.WriteLine(e.Message);
            }
        }

        public void CreateDirectory(string sourceName)
        {
            if (Directory.Exists(sourceName))
            {
                Console.WriteLine("That path exists already.");
                return;
            }
            DirectoryInfo di = Directory.CreateDirectory(sourceName);
        }

        public void FileMove(string sourceName, string destName)
        {
            if (File.Exists(destName))
                File.Delete(destName);
            File.Move(sourceName, destName);
        }

        public void DirectoryMove(string sourceName, string destName)
        {
            try
            {
                Directory.Move(sourceName, destName);
            }
            catch (Exception e)
            {
                fileManager.WriteLine(e.Message);
            }
        }

        public void FileCopy(string sourceName, string destName)
        {
            File.Copy(sourceName, destName, true);
        }

        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }
            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
