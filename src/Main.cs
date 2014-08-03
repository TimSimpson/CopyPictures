using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CopyPictures
{
    class Program
    {
        static void Usage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
@"Usage:
    CopyPictures sourceDirectory destDirectory"
            );
        }

        static bool CheckDir(string dir)
        {
            if (!Directory.Exists(dir)) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("No such directory: " + dir);
                return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            Environment.ExitCode = 1;
            var normal = Console.ForegroundColor;
            if (args.Length < 2) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Incorrect arguments.");
                Usage();
            } else {
                var src = args[0];
                var dest = args[1];
                if (CheckDir(src) && CheckDir(dest)) {
                    var repo = new PictureRepo(dest);
                    repo.AddDirectory(src);
                    Environment.ExitCode = 0;
                }
            }
            Console.ForegroundColor = normal;

        }
    }
}
