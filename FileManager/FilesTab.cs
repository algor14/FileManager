using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace FileManager
{
    public class FilesTab
    {
        private FileManager fileManager;
        private int bottomCoordTabs;
        private int leftOffsetTab;
        private int maxLineLength;
        private int topCoordTabs;
        private DirectoryInfo dir;
        private int MaxShowedLines;
        private bool IsActiveNow = false; // shows is this tab is active tab
        private int CurrentLine; // current index where we are wrighting line now
        private int FromIndex;// start index in the list of all files
        private int ToIndex; // end index in the list of all files
        private int currentElement; //selected element in the list
        private int maxElements;
        private List<FileItem> allFileItems;

        public string Path { get; set; }

        public FilesTab(
            FileManager fileManager,
            string path,
            int topCoordTabs,
            int bottomCoordTabs,
            int leftOffsetTab,
            int maxLineLength)
        {
            this.fileManager = fileManager;
            Path = path;
            this.topCoordTabs = topCoordTabs;
            this.bottomCoordTabs = bottomCoordTabs;
            this.leftOffsetTab = leftOffsetTab;
            this.maxLineLength = maxLineLength;
            dir = new DirectoryInfo(path);
            InitTab();
            ShowTab();
        }

        public FileItem GetFileItem()
        {
            allFileItems[currentElement].FullPath = Path + allFileItems[currentElement].Name;
            return allFileItems[currentElement];
        }

        public void GetFolderInfo(out string size, out int filesNumber, out int foldersNumber)
        {


            DirectoryInfo di = new DirectoryInfo(allFileItems[currentElement].FullPath);
            size = ConstructSize(di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(f => f.Length));
            filesNumber = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Count();
            foldersNumber = di.EnumerateDirectories("*.*", SearchOption.AllDirectories).Count();
        }

        public void UpdateFolder()
        {
            InitTab();
            ShowTab();
        }

        public void EnterFolder(bool isUp = false)
        {
            string oldPath = Path;
            if (!isUp)
            {
                Path += allFileItems[currentElement].Name + @"\";
            }
            try
            {
                dir = new DirectoryInfo(Path);
                dir.GetDirectories();
                InitTab();
                ShowTab();
            }
            catch (Exception e)
            {
                fileManager.WriteLine(e.Message);
                Path = oldPath;
            }
        }

        public void ChangeDrive(string str)
        {
            try
            {
                string oldPath = Path;
                Path = str.ToUpper()[0] + @":\";
                dir = new DirectoryInfo(Path);
                if (dir.Exists)
                {
                    InitTab();
                    ShowTab();
                }
                else
                {
                    Path = oldPath;
                }
            }
            catch(Exception e)
            {
                fileManager.WriteLine(e.Message);
            }
        }

        public void FolderUp()
        {
            if (Path.Count(f => f == '\\') > 1)
            {
                Path = string.Join("", Path.Take(Path.Length - 1).ToArray());
                Path = string.Join("", Path.Take(Path.LastIndexOf(@"\") + 1).ToArray());
                EnterFolder(true);
            }
        }

        public void SwitchActivation()
        {
            IsActiveNow = !IsActiveNow;
            if (IsActiveNow)
                WriteSelectedLine(allFileItems[currentElement], leftOffsetTab, CurrentLine);
            else
                WriteSimpleLine(allFileItems[currentElement], leftOffsetTab, CurrentLine);
        }
        
        private void InitTab()
        {
            
            FromIndex = 0;
            currentElement = 0;
            maxElements = -1;
            CurrentLine = topCoordTabs;
            ToIndex = bottomCoordTabs;
            MaxShowedLines = bottomCoordTabs + 1;
            allFileItems = new List<FileItem>();
            foreach (var item in dir.GetDirectories())
            {
                FileItem fi = new FileItem(allFileItems.Count, item.Name);
                fi.LastReadTime = item.LastAccessTime;
                fi.LastWriteTime = item.LastWriteTime;
                allFileItems.Add(fi);
            }
            foreach (var item in dir.GetFiles())
            {
                FileItem fi = new FileItem(allFileItems.Count, item.Name, item.Extension);
                fi.LastReadTime = item.LastAccessTime;
                fi.LastWriteTime = item.LastWriteTime;
                fi.IsReadOnly = item.Attributes.HasFlag(FileAttributes.ReadOnly);
                fi.Size = ConstructSize(item.Length);
                allFileItems.Add(fi);
            }
            maxElements = allFileItems.Count;
        }

        private void ClearTab()
        {
            for (int CurrentLine = topCoordTabs; CurrentLine < bottomCoordTabs + topCoordTabs + 1; CurrentLine++)
                lock (fileManager.syncObject)
                    WriteClearLine(leftOffsetTab, CurrentLine);
        }

        private void ShowTab()
        {
            ClearTab();
            string str = Path.Length < 40 ? Path : string.Join("", Path.Take(20)) + " ... " + string.Join("", Path.Skip(Path.Length - 20));     //Title
            lock (fileManager.syncObject)
            {
                Console.SetCursorPosition(leftOffsetTab, 2);            // title
                Console.WriteLine(str.PadRight(Console.WindowWidth / 2 - 10));          // title
            }
            int min = MaxShowedLines < maxElements ? MaxShowedLines : maxElements;
            for (int i = 0; i < min; i++)
            {
                if (i == currentElement && IsActiveNow)
                    WriteSelectedLine(allFileItems[i], leftOffsetTab, CurrentLine);
                else
                    WriteSimpleLine(allFileItems[i], leftOffsetTab, CurrentLine);
                CurrentLine++;
            }
            CurrentLine = topCoordTabs;
        }

        public string ConstructSize(long x)
        {
            string resultSize;
            if (x < 1000)
                resultSize = (x.ToString() + " bytes");
            else if (x < 1000000)
                resultSize = ((x / 1000).ToString() + " kb");
            else
                resultSize = ((x / 1000000).ToString() + " Mb");
            return resultSize;
        }

        private void WriteClearLine(int x, int y)
        {
            lock (fileManager.syncObject)
            {
                string value = "";
                Console.SetCursorPosition(x, y);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(value.PadRight(Console.WindowWidth / 3 + 10));
            }
        }

        private void WriteSimpleLine(FileItem item, int x, int y)
        {
            string value = item.Name;
            if (value.Length > maxLineLength)
                value = string.Join("", value.Take(25).ToArray()) + "[...]";
            lock (fileManager.syncObject)
            {
                Console.SetCursorPosition(x + 35, y);
                Console.WriteLine(item.Extension);
                Console.SetCursorPosition(x, y);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine(value.PadRight(Console.WindowWidth / 3 + 10));
                Console.SetCursorPosition(x + 35, y);
                Console.WriteLine(item.Extension);
                Console.SetCursorPosition(x + 45, y);
                if (item.Extension != "dir")
                    Console.WriteLine(item.Size);
            }
        }

        private void WriteSelectedLine(FileItem item, int x, int y)
        {
            string value = item.Name;
            if (value.Length > maxLineLength)
                value = string.Join("", value.Take(25).ToArray()) + "[...]";
            lock (fileManager.syncObject)
            {
                Console.SetCursorPosition(x, y);
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(value.PadRight(Console.WindowWidth / 3 + 10)); // <-- see note
                Console.SetCursorPosition(x + 35, y);
                Console.WriteLine(item.Extension);
                Console.SetCursorPosition(x + 45, y);
                if (item.Extension != "dir")
                    Console.WriteLine(item.Size);
                Console.ResetColor();
            }
        }

        public void MoveCursor(bool isDown = true)
        {
            if (isDown)
            {
                if (currentElement - FromIndex < bottomCoordTabs && currentElement < maxElements - 1)
                {
                    WriteSimpleLine(allFileItems[currentElement], leftOffsetTab, CurrentLine);
                    WriteSelectedLine(allFileItems[++currentElement], leftOffsetTab, ++CurrentLine);
                }
                else
                {
                    MoveTabDown();
                }
            }
            else
            {
                if (currentElement - FromIndex > 0)
                {
                    WriteSimpleLine(allFileItems[currentElement], leftOffsetTab, CurrentLine);
                    WriteSelectedLine(allFileItems[--currentElement], leftOffsetTab, --CurrentLine);
                }
                else
                {
                    MoveTabUp();
                }
            }
        }

        private void MoveTabDown()
        {
            if (ToIndex < maxElements - 1)
            {
                FromIndex++;
                ToIndex++;
                CurrentLine = topCoordTabs;
                currentElement++;
                for (int i = FromIndex; i <= ToIndex; i++)
                {
                    if (i == currentElement)
                        WriteSelectedLine(allFileItems[i], leftOffsetTab, CurrentLine);
                    else
                    {
                        WriteSimpleLine(allFileItems[i], leftOffsetTab, CurrentLine);
                        CurrentLine++;
                    }
                }
            }
        }

        private void MoveTabUp()
        {
            if (FromIndex > 0)
            {
                FromIndex--;
                ToIndex--;
                currentElement--;
                CurrentLine = topCoordTabs;
                for (int i = FromIndex; i <= ToIndex; i++)
                {
                    if (i == currentElement)
                        WriteSelectedLine(allFileItems[i], leftOffsetTab, CurrentLine);
                    else
                        WriteSimpleLine(allFileItems[i], leftOffsetTab, CurrentLine);
                    CurrentLine++;
                }
                CurrentLine = topCoordTabs;
            }
        }
    }
}
