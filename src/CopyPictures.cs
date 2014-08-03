﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging; // PropertyItem
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;  // BitmapMetaData

namespace CopyPictures
{


public class PictureInfo
{
    public readonly DateTime DateTaken;
    public readonly string FilePath;
    public readonly DateTime FileModTime;
    public readonly long Length;

    public PictureInfo(string filePath)
    {
        this.FilePath = filePath;
        this.FileModTime = File.GetLastWriteTime(this.FilePath);
        this.Length = new FileInfo(this.FilePath).Length;
        try
        {
            using (var stream = new FileStream(this.FilePath, FileMode.Open,
                                               FileAccess.Read)
                  )
            {
                BitmapSource bs = BitmapFrame.Create(stream);
                BitmapMetadata metadata = (BitmapMetadata) bs.Metadata;
                this.DateTaken = DateTime.Parse(metadata.DateTaken);
            }
        }
        catch(Exception)
        {
            this.DateTaken = FileModTime;
        }
    }

    public void PrintVerboseInfo()
    {
        Console.WriteLine("\t" + this.FilePath);
        var modTime = File.GetLastWriteTime(this.FilePath);
        var length = new FileInfo(this.FilePath).Length;
        Console.WriteLine("\t\tSize: " + this.Length);
        Console.WriteLine("\t\tDate Taken    : {0:yyy-MM-dd HH:mm:ss}",
                          this.DateTaken);
        Console.WriteLine("\t\tFile Mode Time: {0:yyy-MM-dd HH:mm:ss}",
                         this.FileModTime);
    }

    public bool IsProbablyTheSameAs(PictureInfo other)
    {
        return other.DateTaken == this.DateTaken
            && other.Length == this.Length;
    }

}

public class PictureRepo
{
    private readonly string dir;
    private readonly ConsoleColor normalColor;

    public PictureRepo(string dir)
    {
        this.dir = dir;
        this.normalColor = Console.ForegroundColor;
    }

    public void AddDirectory(string srcDir)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Iterating " + srcDir + "...");
        Console.ForegroundColor = normalColor;
        foreach (string dir in Directory.GetDirectories(srcDir))
        {
            AddDirectory(dir);
        }
        foreach (string file in Directory.GetFiles(srcDir))
        {
            AddFile(file);
        }
    }

    public void AddFile(string srcFile)
    {
        var srcInfo = new PictureInfo(srcFile);

        var dstDir = findOrCreateDirectory(srcInfo.DateTaken);

        var nameTag = string.Format("{0:dd-HH_mm_ss}", srcInfo.DateTaken);
        var fileName = Path.GetFileName(srcInfo.FilePath);
        var dstFile = dstDir + "\\" + nameTag + "-" + fileName;

        int copyCount = 0;
        while (File.Exists(dstFile)) {
            copyCount ++;
            dstFile = dstDir + "\\Copy" + copyCount + "_"
                    + nameTag + "-" + fileName;
        }
        copyFile(srcInfo, dstFile);
    }

    private bool chooseToOverwrite(PictureInfo src, PictureInfo dst)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("We want to copy:");
        Console.ForegroundColor = ConsoleColor.Green;
        src.PrintVerboseInfo();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\t  -- to --> ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        dst.PrintVerboseInfo();
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

    private void copyFile(PictureInfo srcInfo, string dstFile)
    {
        Console.WriteLine("  src << " + srcInfo.FilePath);
        Console.WriteLine("  dst  >> " + dstFile);

        bool overwrite = false;
        if (File.Exists(dstFile))
        {
            var dstInfo = new PictureInfo(dstFile);
            if (!chooseToOverwrite(srcInfo, dstInfo))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Skipping.");
                Console.ForegroundColor = normalColor;
                return;
            }
            else
            {
                overwrite = true;
            }
        }
        File.Copy(srcInfo.FilePath, dstFile, overwrite);
        File.SetLastWriteTime(dstFile, srcInfo.DateTaken);
    }

    private string findOrCreateDirectory(DateTime time)
    {
        var yearDir = string.Format("{0:yyyy}", time);
        var monthDir = string.Format("{0:MM}", time);

        var dir1 = dir + "\\" + yearDir;
        var dir2 = dir1 + "\\" + monthDir;

        if (!Directory.Exists(dir1)) {
            Directory.CreateDirectory(dir1);
        }
        if (!Directory.Exists(dir2)) {
            Directory.CreateDirectory(dir2);
        }

        return dir2;
    }


}
}
