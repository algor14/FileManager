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
    class FileManager
    {
        private const int TopCoordTabs = 5;
        private const int BottomCoordTabs = 30;
        private const int MaxLineLength = 20;
        private const int MaxShowedLines = BottomCoordTabs + 1;
        private const int LeftOffsetTab1 = 5;
        private const int LeftOffsetTab2 = 75;

        private string path1 = @"C:\";
        private string path2 = @"C:\";
        private int PressesAllowedPause = 10; // helps to avoid wrong enter presses
        private FilesTab tab1;
        private FilesTab tab2;
        private FilesTab currentTab;
        private FileItem buffer = null;
        private FilesProcessor fileProcessor;

        public FileManager()
        {
            fileProcessor = new FilesProcessor(this);
            tab1 = new FilesTab(path1, TopCoordTabs, BottomCoordTabs, LeftOffsetTab1, MaxLineLength);
            tab2 = new FilesTab(path2, TopCoordTabs, BottomCoordTabs, LeftOffsetTab2, MaxLineLength);
            currentTab = tab1;
            currentTab.SwitchActivation();
            DrawMenu();
            Update();
        }

        private void Update()
        {
            while (true)
            {
                CheckPressedKeys();
                Thread.Sleep(50);
            }
        }

        private void DrawMenu()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 140; i++)
                Console.Write("=");
            Console.SetCursorPosition(0, 3);
            for (int i = 0; i < 140; i++)
                Console.Write("=");
            for (int i = 0; i < 47; i++)
            {
                Console.SetCursorPosition(68, i);
                Console.Write("||");
            }
            for (int i = 0; i < 49; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("||");
            }
            for (int i = 0; i < 49; i++)
            {
                Console.SetCursorPosition(138, i);
                Console.Write("||");
            }
            Console.SetCursorPosition(0, 47);
            for (int i = 0; i < 140; i++)
                Console.Write("=");
            Console.SetCursorPosition(0, 49);
            for (int i = 0; i < 140; i++)
                Console.Write("=");
            Console.SetCursorPosition(0, 52);
            for (int i = 0; i < 140; i++)
                Console.Write("=");
            Console.SetCursorPosition(0, 37);
            for (int i = 0; i < 140; i++)
                Console.Write("=");
            Console.SetCursorPosition(0, 50);
            Console.Write("     F1 - Copy   |   F2 - Cut   |   F3 - Paste   |   F4 - List Of Disks   |   F5 - Properties   |   F6 - Rename   |   F7 - New Folder");
            WriteLine();
        }

        public void CheckPressedKeys()
        {
            if (PressesAllowedPause > 0)
            {
                PressesAllowedPause--;
                return;
            }
            if (Input.IsKeyDown(Keys.DOWN))
                MoveCursor();
            if (Input.IsKeyDown(Keys.UP))
                MoveCursor(false);
            if (Input.IsKeyDown(Keys.RETURN))
                EnterFolder();
            if (Input.IsKeyDown(Keys.TAB))
                ChangeTab();
            if (Input.IsKeyDown(Keys.BACK))
                FolderUp();
            if (Input.IsKeyDown(Keys.F1))
                CopyCutFile();
            if (Input.IsKeyDown(Keys.F2))
                CopyCutFile(true);
            if (Input.IsKeyDown(Keys.F3))
                PasteFile();
            if (Input.IsKeyDown(Keys.F4))
                ChangeDrive();
            if (Input.IsKeyDown(Keys.F5))
                GetProperties();
            if (Input.IsKeyDown(Keys.F6))
                Rename();
            if (Input.IsKeyDown(Keys.F7))
                CreateDirectory();
        }

        private void KeyWasPressed()
        {
            PressesAllowedPause = 10;
        }

        private void MoveCursor(bool isDown = true)
        {
            currentTab.MoveCursor(isDown);
        }

        private void GetProperties()
        {
            ClearPropertyWindow();
            FileItem fi = currentTab.GetFileItem();
            WritePropery(0, "Name:", fi.Name);
            WritePropery(1, "Parent directory:", currentTab.Path);
            WritePropery(2, "Root directory:", string.Join("", currentTab.Path.Take(3)));
            WritePropery(3, "Last read time:", fi.LastReadTime.ToString());
            WritePropery(4, "Last write time:", fi.LastWriteTime.ToString());
            if (fi.Extension == "dir")
            {
                string dirSize;
                int filesNumber;
                int foldersNumber;
                currentTab.GetFolderInfo(out dirSize, out filesNumber, out foldersNumber);
                WritePropery(5, "Size:", dirSize);
                WritePropery(6, "Files:", filesNumber.ToString());
                WritePropery(7, "Folders:", foldersNumber.ToString());
            }
            else
            {
                WritePropery(5, "Is read only:", fi.IsReadOnly.ToString());
                WritePropery(6, "Size:", fi.Size);
            }
        }

        private void WritePropery(int propIndex, string propName, string propValue)
        {
            int posX;
            posX = currentTab == tab2 ? 72 : 3;
            Console.SetCursorPosition(posX, 38 + propIndex);
            Console.Write(propName);
            Console.SetCursorPosition(posX + 20, 38 + propIndex);
            Console.Write(propValue);
        }

        private void ChangeDrive()
        {
            KeyWasPressed();
            ClearPropertyWindow();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            int posX;
            posX = currentTab == tab2 ? 72 : 3;
            for (int i = 0; i < allDrives.Length; i++)
            {
                Console.SetCursorPosition(posX, 38 + i);
                Console.WriteLine($"Drive {allDrives[i].Name}");
            }
            WriteLine("Enter letter to select the drive: ");
            currentTab.ChangeDrive(Console.ReadLine());
            ClearPropertyWindow();
            KeyWasPressed();
        }

        private void ClearPropertyWindow()
        {
            for (int i = 0; i < 8; i++)
            {
                Console.SetCursorPosition(3, 38 + i);
                Console.WriteLine("".PadRight(Console.WindowWidth / 2 - 20));
                Console.SetCursorPosition(72, 38 + i);
                Console.WriteLine("".PadRight(Console.WindowWidth / 2 - 20));
            }
        }

        private void CreateDirectory()
        {
            KeyWasPressed();
            WriteLine("Enter forlder name:");
            Console.SetCursorPosition(28, 48);
            string str = Console.ReadLine();
            KeyWasPressed();
            fileProcessor.CreateDirectory(currentTab.Path + str);
            currentTab.UpdateFolder();
        }

        private void Rename()
        {
            KeyWasPressed();
            WriteLine("Enter new name:");
            Console.SetCursorPosition(23, 48);
            string str = Console.ReadLine();
            KeyWasPressed();
            if (currentTab.GetFileItem().Extension == "dir")
                fileProcessor.DirectoryRename(currentTab.Path + currentTab.GetFileItem().Name, currentTab.Path + str);
            else
                fileProcessor.FileRename(currentTab.Path + currentTab.GetFileItem().Name, currentTab.Path + str + currentTab.GetFileItem().Extension);
            currentTab.UpdateFolder();
        }

        private void EnterFolder()
        {
            KeyWasPressed();
            currentTab.EnterFolder();
        }

        private void FolderUp()
        {
            KeyWasPressed();
            currentTab.FolderUp();
        }

        private void ChangeTab()
        {
            KeyWasPressed();
            currentTab.SwitchActivation();
            currentTab = currentTab == tab1 ? tab2 : tab1;
            currentTab.SwitchActivation();
        }

        private void CopyCutFile(bool isCutted = false)
        {
            fileProcessor.IsCutted = isCutted;
            buffer = currentTab.GetFileItem();
        }

        private void PasteFile()
        {
            KeyWasPressed();
            if (fileProcessor.IsCutted)
            {
                if (buffer.Extension == "dir")
                    fileProcessor.DirectoryMove(buffer.FullPath, currentTab.Path + buffer.Name);
                else
                    fileProcessor.FileMove(buffer.FullPath, (currentTab.Path + buffer.Name));
            }
            else
            {
                if (buffer.Extension == "dir")
                    fileProcessor.DirectoryCopy(buffer.FullPath, currentTab.Path + buffer.Name, true);
                else
                    fileProcessor.FileCopy(buffer.FullPath, (currentTab.Path + buffer.Name));
            }
            tab1.UpdateFolder();
            tab2.UpdateFolder();
        }

        public void ClearLine(string str = "")
        {
            Console.SetCursorPosition(2, 48);
            Console.WriteLine("".PadRight(Console.WindowWidth - 30));
        }

        public void WriteLine(string str = "")
        {
            ClearLine();
            Console.SetCursorPosition(2, 48);
            Console.Write(">>>>> ");
            Console.Write(str);
        }

    }
}
