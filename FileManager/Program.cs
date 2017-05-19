using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class Program
    {
        private static FileManager fmanager;
        static void Main(string[] args)
        {
            Console.WindowWidth = 140;
            Console.WindowHeight = 54;
            //Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.Clear();
            fmanager = new FileManager();
        }
    }
}
