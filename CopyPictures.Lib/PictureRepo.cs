using System;
using System.IO;

namespace CopyPictures
{
/// <summary>
/// Represents a "repository" of pictures- which is really just a directory.
/// </summary>
public class PictureRepo
{
    private readonly string dir;
    private readonly bool dryRun;
    private readonly PictureDuplicateOptions dupeOptions;
    private readonly ConsoleColor normalColor;

    public PictureRepo(string dir, PictureDuplicateOptions dupeOptions,
                       bool dryRun)
    {
        this.dir = dir;
        this.dryRun = dryRun;
        this.normalColor = Console.ForegroundColor;
        this.dupeOptions = dupeOptions;
    }

    /// <summary>
    /// Iterates the given directory, finds all pictures and copies them to 
    /// the "repo" directory.
    /// </summary>
    /// <param name="srcDir">The directory to add.</param>
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

    /// <summary>
    /// Given a single file, adds it to the "picture repo" directory. The
    /// filename will be the original filename, prepended with the day and
    /// time the picture was taken. The directory will be the year and month
    /// the picture was taken.
    /// </summary>
    /// <param name="srcFile">File to add.</param>
    public void AddFile(string srcFile)
    {
        var srcInfo = new PictureInfo(srcFile);

        var dstDir = findOrCreateDirectory(srcInfo.DateTaken);

        var nameTag = string.Format("{0:dd-HH_mm_ss}", srcInfo.DateTaken);
        var fileName = Path.GetFileName(srcInfo.FilePath);
        var dstFile = dstDir + "\\" + nameTag + "-" + fileName;

        if (this.dupeOptions
            == PictureDuplicateOptions.CopyWithSlightlyDifferentFileName)
        {
            int copyCount = 0;
            while (File.Exists(dstFile)) {
                copyCount ++;
                dstFile = dstDir + "\\Copy" + copyCount + "_"
                        + nameTag + "-" + fileName;
            }
        }
        copyFile(srcInfo, dstFile);
    }

    private bool chooseToOverwrite(PictureInfo src, PictureInfo dst)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("We want to copy:");
        Console.ForegroundColor = ConsoleColor.Green;
        src.PrintVerboseInfo(dst);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\t  -- to --> ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        dst.PrintVerboseInfo(src);
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

    /// <summary>
    /// Actually copies a picture to a destination file. May prompt if 
    /// needed.
    /// </summary>
    /// <param name="srcInfo">Picture to copy.</param>
    /// <param name="dstFile">Destination file path.</param>
    private void copyFile(PictureInfo srcInfo, string dstFile)
    {
        Console.WriteLine("  src << " + srcInfo.FilePath);
        Console.WriteLine("  dst  >> " + dstFile);

        bool overwrite = false;
        if (File.Exists(dstFile))
        {
            if (this.dupeOptions != PictureDuplicateOptions.PromptToOverwrite
                && this.dupeOptions != PictureDuplicateOptions.PromptButNotIfTimesMatchAndDestIsSmaller)
            {
                throw new Exception("Bug!");
            }
            var dstInfo = new PictureInfo(dstFile);
            bool summonPrompt = true;
            if (dupeOptions == PictureDuplicateOptions.PromptButNotIfTimesMatchAndDestIsSmaller)
            {
                if (srcInfo.DateTaken == dstInfo.DateTaken)
                {
                    if (srcInfo.Length > dstInfo.Length)
                    {
                        overwrite = true;
                        summonPrompt = true;  // TODO: Change back
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   src is bigger. REPLACING.");
                        Console.ForegroundColor = normalColor;
                    }
                    else if (srcInfo.Length <= dstInfo.Length)
                    {
                        overwrite = false;
                        summonPrompt = false;
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (srcInfo.Length == dstInfo.Length)
                        {
                            Console.WriteLine("                            dest is the SAME SIZE. !! Skipping.");
                        }
                        else
                        {
                            Console.WriteLine("                            dest is BIGGER?! Skipping.");
                        }

                        Console.ForegroundColor = normalColor;
                    }
                }
            }
            if (summonPrompt)
            {
                overwrite = chooseToOverwrite(srcInfo, dstInfo);
            }
            if (!overwrite)
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

        if (dryRun == true)
        {
            return;
        }
        File.Copy(srcInfo.FilePath, dstFile, overwrite);
        File.SetLastWriteTime(dstFile, srcInfo.DateTaken);
    }

    /// <summary>
    /// Ensures the directory needed to store pics from a given time exists
    /// and returns it's path.
    /// </summary>
    /// <param name="time">The time (only the year and month are used).</param>
    /// <returns>Path to the directory.</returns>
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
