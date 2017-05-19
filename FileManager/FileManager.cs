using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NConsoleGraphics;
using System.Threading;

namespace FileManager
{

    class FilesTab2
    {
        private int bottomCoordTabs;
        private int leftOffsetTab;
        private string path;
        private int topCoordTabs;
        private DirectoryInfo dir;

        private int MaxShowedLines;

        private int BlockIndexCurrentLine; // current index where we wrighting line now
        private int FromIndex = 0;// start index in the list of all files
        private int ToIndex; // end index in the list of all files
        public int currentElement = 0; //current element in the list
        private int maxElements = -1;

        private List<DirectoryInfo> foldersTab1;
        private List<FileInfo> filesTab1;
        private List<string> allMembers;

        public FilesTab2()
        {

        }

        public FilesTab2(string path, int topCoordTabs, int bottomCoordTabs, int leftOffsetTab)
        {
            this.path = path;
            this.topCoordTabs = topCoordTabs;
            this.bottomCoordTabs = bottomCoordTabs;
            this.leftOffsetTab = leftOffsetTab;
            dir = new DirectoryInfo(path);
            InitTab();
            ShowTab();
        }

        private void InitTab()
        {
            BlockIndexCurrentLine = topCoordTabs;
            ToIndex = bottomCoordTabs;
            MaxShowedLines = bottomCoordTabs + 1;

            allMembers = new List<string>();
            foreach (var item in dir.GetDirectories())
            {
                //foldersTab1.Add(item);
                allMembers.Add(item.Name);
            }
            foreach (var item in dir.GetFiles())
            {
                //filesTab1.Add(item);
                allMembers.Add(item.Name);
            }
            maxElements = allMembers.Count;
        }
        public void ClearTab()
        {
            for (int BlockIndexCurrentLine = 0; BlockIndexCurrentLine < MaxShowedLines; BlockIndexCurrentLine++)
            {
                WriteSimpleLine("", leftOffsetTab, BlockIndexCurrentLine);
            }
        }
        public void ShowTab()
        {
            //Console.Clear();
            ClearTab();
            //int index = 0;
            for (int i = 0; i < MaxShowedLines; i++)
            {
                if (i == currentElement)
                    WriteSelectedLine(allMembers[i], leftOffsetTab, BlockIndexCurrentLine);
                else
                    WriteSimpleLine(allMembers[i], leftOffsetTab, BlockIndexCurrentLine);
                BlockIndexCurrentLine++;
                //index++;
            }
            BlockIndexCurrentLine = topCoordTabs;
            //foreach (var item in foldersTab1)
            //{
            //    if (index == currentLine)
            //        WriteSelectedLine(item.Name);
            //    else
            //        WriteSimpleLine(item.Name);
            //    index++;
            //}

            //foreach (var item in filesTab1)
            //{
            //    if (index == currentLine)
            //        WriteSelectedLine(item.Name);
            //    else
            //        WriteSimpleLine(item.Name);
            //    index++;
            //}
        }

        void WriteSimpleLine(string value, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = ConsoleColor.Black;
            //Console.Write("   ");
            Console.WriteLine(value.PadRight(Console.WindowWidth / 3));
        }

        void WriteSelectedLine(string value, int x, int y)
        {
            //
            // This method writes an entire line to the console with the string.
            //
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            //Console.Write("   ");
            Console.WriteLine(value.PadRight(Console.WindowWidth / 3)); // <-- see note

            Console.ResetColor();
        }

        public void MoveCursor(bool isDown = true)
        {
            if (isDown)
            {
                if (currentElement - FromIndex < bottomCoordTabs)
                {
                    WriteSimpleLine(allMembers[currentElement + FromIndex], leftOffsetTab, BlockIndexCurrentLine);
                    WriteSelectedLine(allMembers[++currentElement + FromIndex], leftOffsetTab, ++BlockIndexCurrentLine);
                }
                else
                {
                    MoveTab();
                }
            }
            else
            {
                if (currentElement - FromIndex > 0)
                {
                    WriteSimpleLine(allMembers[currentElement], leftOffsetTab, BlockIndexCurrentLine);
                    WriteSelectedLine(allMembers[--currentElement], leftOffsetTab, --BlockIndexCurrentLine);
                }
                else
                {
                    MoveTab(false);
                }
            }
        }

        public void MoveTab(bool isDown = true)
        {
            if (isDown)
            {
                if (ToIndex < maxElements - 1)
                {
                    FromIndex++;
                    ToIndex++;
                    BlockIndexCurrentLine = topCoordTabs;
                    currentElement++;
                    for (int i = FromIndex; i <= ToIndex; i++)
                    {
                        if (i == currentElement)
                            WriteSelectedLine(allMembers[i], leftOffsetTab, BlockIndexCurrentLine);
                        else
                        {
                            WriteSimpleLine(allMembers[i], leftOffsetTab, BlockIndexCurrentLine);
                            BlockIndexCurrentLine++;
                        }
                    }
                }
            }
            else
            {
                if (FromIndex > 0)
                {
                    FromIndex--;
                    ToIndex--;
                    currentElement--;
                    BlockIndexCurrentLine = topCoordTabs;
                    for (int i = FromIndex; i <= ToIndex; i++)
                    {
                        if (i == currentElement)
                            WriteSelectedLine(allMembers[i], leftOffsetTab, BlockIndexCurrentLine);
                        else
                            WriteSimpleLine(allMembers[i], leftOffsetTab, BlockIndexCurrentLine);
                        BlockIndexCurrentLine++;
                    }
                    BlockIndexCurrentLine = topCoordTabs;
                }
            }
        }
    }




















    class FileManager
    {
        private const int TopCoordTabs = 5;
        private const int BottomCoordTabs = 30;


        private const int MaxShowedLines = BottomCoordTabs + 1;

        private int BlockIndexCurrentLine = TopCoordTabs; // current index where we wrighting line now
        private int FromIndex = 0;// start index in the list of all files
        private int ToIndex = BottomCoordTabs; // end index in the list of all files

        private const int LeftOffsetTab1 = 5;
        private const int LeftOffsetTab2 = 75;

        private string path1 = @"C:\";
        private string path2 = @"D:\";
        private ConsoleGraphics graphics = new ConsoleGraphics();
        public int currentElement = 0; //current element in the list
        private int maxElements = -1;
        private DirectoryInfo dir;

        private List<DirectoryInfo> foldersTab1;
        private List<FileInfo> filesTab1;
        private List<string> allMembers;

        private FilesTab2 tab1;
        private FilesTab2 tab2;
        public FileManager()
        {

            //dir = new DirectoryInfo(path2);
            //Console.SetWindowPosition(0, 0);
            //foldersTab1 = new List<DirectoryInfo>();
            //filesTab1 = new List<FileInfo>();
            //allMembers = new List<string>();
            //InitTab();
            //ShowTab();
            tab1 = new FilesTab2(path2, TopCoordTabs, BottomCoordTabs, LeftOffsetTab1);
            tab2 = new FilesTab2(path2, TopCoordTabs, BottomCoordTabs, LeftOffsetTab2);

            Update();
        }
        public void Update()
        {
            while (true)
            {
                graphics.FillRectangle(0xFFFFFFFF, graphics.ClientWidth / 2, 20, 10, graphics.ClientHeight + 20);
                CheckPressedKeys();

                graphics.FlipPages();
                Thread.Sleep(50);
            }
        }

        public void CheckPressedKeys()
        {
            if (Input.IsKeyDown(Keys.DOWN))
            {
                MoveCursor2();
            }
            if (Input.IsKeyDown(Keys.UP))
            {
                MoveCursor2(false);
            }
            if (Input.IsKeyDown(Keys.RETURN))
            {
                EnterFolder();
            }
            if (Input.IsKeyDown(Keys.TAB))
            {
                //EnterFolder();
            }
        }

        public void MoveCursor2()
        {
            сделать выбор между табами, кому отправлять Move
                табы сделаны уже через класс, можно убирать все из основного
        }

        public void EnterFolder()
        {
            dir = new DirectoryInfo(path2 + allMembers[currentElement]);
            allMembers = new List<string>();
            InitTab();
            ShowTab();
        }




        public void InitTab()
        {
            foreach (var item in dir.GetDirectories())
            {
                foldersTab1.Add(item);
                allMembers.Add(item.Name);
            }
            foreach (var item in dir.GetFiles())
            {
                filesTab1.Add(item);
                allMembers.Add(item.Name);
            }
            maxElements = foldersTab1.Count + filesTab1.Count;
        }







        public void MoveCursor(bool isDown = true)
        {
            if (isDown)
            {
                if (currentElement - FromIndex < BottomCoordTabs)
                {
                    WriteSimpleLine(allMembers[currentElement + FromIndex], LeftOffsetTab1, BlockIndexCurrentLine);
                    WriteSelectedLine(allMembers[++currentElement + FromIndex], LeftOffsetTab1, ++BlockIndexCurrentLine);
                }
                else
                {
                    MoveTab();
                }
            }
            else
            {
                if (currentElement - FromIndex > 0)
                {
                    WriteSimpleLine(allMembers[currentElement], LeftOffsetTab1, BlockIndexCurrentLine);
                    WriteSelectedLine(allMembers[--currentElement], LeftOffsetTab1, --BlockIndexCurrentLine);
                }
                else
                {
                    MoveTab(false);
                }
            }
        }

        public void MoveTab(bool isDown = true)
        {
            if (isDown)
            {
                if (ToIndex < maxElements - 1)
                {
                    FromIndex++;
                    ToIndex++;
                    BlockIndexCurrentLine = TopCoordTabs;
                    currentElement++;
                    for (int i = FromIndex; i <= ToIndex; i++)
                    {
                        if (i == currentElement)
                            WriteSelectedLine(allMembers[i], LeftOffsetTab1, BlockIndexCurrentLine);
                        else
                        {
                            WriteSimpleLine(allMembers[i], LeftOffsetTab1, BlockIndexCurrentLine);
                            BlockIndexCurrentLine++;
                        }
                    }
                }
            }
            else
            {
                if (FromIndex > 0)
                {
                    FromIndex--;
                    ToIndex--;
                    currentElement--;
                    BlockIndexCurrentLine = TopCoordTabs;
                    for (int i = FromIndex; i <= ToIndex; i++)
                    {
                        if (i == currentElement)
                            WriteSelectedLine(allMembers[i], LeftOffsetTab1, BlockIndexCurrentLine);
                        else
                            WriteSimpleLine(allMembers[i], LeftOffsetTab1, BlockIndexCurrentLine);
                        BlockIndexCurrentLine++;
                    }
                    BlockIndexCurrentLine = TopCoordTabs;
                }
            }
        }




        public void ShowTab()
        {
            Console.Clear();
            //int index = 0;
            for (int i = 0; i < MaxShowedLines; i++)
            {
                if (i == currentElement)
                    WriteSelectedLine(allMembers[i], LeftOffsetTab1, BlockIndexCurrentLine);
                else
                    WriteSimpleLine(allMembers[i], LeftOffsetTab1, BlockIndexCurrentLine);
                BlockIndexCurrentLine++;
                //index++;
            }
            BlockIndexCurrentLine = TopCoordTabs;
            //foreach (var item in foldersTab1)
            //{
            //    if (index == currentLine)
            //        WriteSelectedLine(item.Name);
            //    else
            //        WriteSimpleLine(item.Name);
            //    index++;
            //}

            //foreach (var item in filesTab1)
            //{
            //    if (index == currentLine)
            //        WriteSelectedLine(item.Name);
            //    else
            //        WriteSimpleLine(item.Name);
            //    index++;
            //}
        }

        void WriteSimpleLine(string value, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = ConsoleColor.Black;
            //Console.Write("   ");
            Console.WriteLine(value.PadRight(Console.WindowWidth / 3));
        }

        void WriteSelectedLine(string value, int x, int y)
        {
            //
            // This method writes an entire line to the console with the string.
            //
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            //Console.Write("   ");
            Console.WriteLine(value.PadRight(Console.WindowWidth / 3)); // <-- see note
                                                                        //
                                                                        // Reset the color.
                                                                        //
            Console.ResetColor();
        }
    }
}
