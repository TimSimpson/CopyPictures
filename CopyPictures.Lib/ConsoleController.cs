using System;
using System.IO;

namespace CopyPictures
{
public class ConsoleController : Output
{
    private readonly ConsoleColor normalColor;

    public ConsoleController()
    {
        this.normalColor = Console.ForegroundColor;
    }

    public bool ChooseToOverwrite(PictureInfo src, PictureInfo dst)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("We want to copy:");
        Console.ForegroundColor = ConsoleColor.Green;
        printVerboseInfo(src, dst);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\t  -- to --> ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        printVerboseInfo(dst, src);
        Console.ForegroundColor = ConsoleColor.Cyan;

        if (dst.IsProbablyTheSameAs(src))
        {
            Console.WriteLine("Yet the destination file looks to be the same "
                                + "already.");
        }
        else
        {
            Console.WriteLine("But the destination file already exists and "
                                + "looks different.");
        }
        Console.WriteLine("Should we copy and overwrite the file anyway?");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("(y/n) ");
        Console.ForegroundColor = normalColor;
        var yn = Console.ReadLine();
        return yn == "y";
    }
   
    public void LogDirectoryItr(string dir)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Iterating " + dir + "...");
        Console.ForegroundColor = normalColor;
    }

    public void LogFileCopy(FileCopyItem fci, bool skipped)
    {
        Console.WriteLine("  src << " + fci.Source.FilePath);
        Console.WriteLine("  dst  >> " + fci.DstFilePath);
        switch(fci.Status)
        {
                case FileCopyStatus.DestinationIsBigger:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("                            < dest is BIGGER");
                    Console.ForegroundColor = normalColor;
                    break;
                case FileCopyStatus.SourceIsBigger:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("                            > src is bigger.");
                    Console.ForegroundColor = normalColor;
                    break;
                case FileCopyStatus.SeeminglyIdentical:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("                            ==? both files are identical?");
                    Console.ForegroundColor = normalColor;
                    break;
                case FileCopyStatus.DifferentTimes:
                    var oldBg = Console.BackgroundColor;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("                            TIMES DO NOT MATCH");
                    Console.ForegroundColor = normalColor;
                    Console.BackgroundColor = oldBg;
                    break;
                case FileCopyStatus.DestinationDoesNotExist:
                break;
        }
        if (skipped)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("                            Skipping.");
            Console.ForegroundColor = normalColor;
        }
    }

    /// <summary>
    /// Given another PictureInfo (another file) prints out details. This
    /// is used to prompt the user to see if they want to overwrite one
    /// file or the other.
    /// </summary>
    /// <param name="other">The other file.</param>
    private void printVerboseInfo(PictureInfo a, PictureInfo b)
    {
        bool bigger = b != null && (a.Length > b.Length);

        Console.WriteLine("\t" + a.FilePath);
        var modTime = File.GetLastWriteTime(a.FilePath);
        var length = new FileInfo(a.FilePath).Length;
        Console.Write("\t\tSize: " + a.Length);
        if (bigger)
        {
            Console.Write("\t\t<-- BIGGER!!!");
        }
        Console.WriteLine();
        Console.WriteLine("\t\tDate Taken    : {0:yyy-MM-dd HH:mm:ss}",
                            a.DateTaken);
        Console.WriteLine("\t\tFile Mode Time: {0:yyy-MM-dd HH:mm:ss}",
                            a.FileModTime);
    }
}
}
