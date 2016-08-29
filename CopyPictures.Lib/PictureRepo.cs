using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CopyPictures
{
/// <summary>
/// Represents a "repository" of pictures- which is really just a directory.
/// </summary>
public class PictureRepo
{
    private readonly string dir;
    private readonly PictureDuplicateOptions dupeOptions;    

    public PictureRepo(string dir, PictureDuplicateOptions dupeOptions)
    {
        this.dir = dir;
        this.dupeOptions = dupeOptions;
    }

    /// <summary>
    /// Iterates the given directory, finds all pictures and copies them to 
    /// the "repo" directory.
    /// </summary>
    /// <param name="srcDir">The directory to add.</param>
    /// <returns>A list of FileCopyItems representing what to do.</returns>
    public List<FileCopyItem> AddDirectory(string srcDir, 
                                           Output output)
    {
        output.LogDirectoryItr(srcDir);
        List<FileCopyItem> list = new List<FileCopyItem>();
        foreach (string dir in Directory.GetDirectories(srcDir))
        {
            list.AddRange(AddDirectory(dir, output));
        }
        foreach (string file in Directory.GetFiles(srcDir))
        {
            list.Add(AddFile(file));
        }
        return list;
    }

    /// <summary>
    /// Given a single file, adds it to the "picture repo" directory. The
    /// filename will be the original filename, prepended with the day and
    /// time the picture was taken. The directory will be the year and month
    /// the picture was taken.
    ///
    /// !! May create new directories.
    /// 
    /// </summary>
    /// <param name="srcFile">File to add.</param>
    /// <returns>A file copy item representing the desired action.</returns>
    public FileCopyItem AddFile(string srcFile)
    {
        var srcInfo = new PictureInfo(srcFile);

        var dstDir = findOrCreateDirectory(srcInfo.DateTaken);

        var nameTag = string.Format("{0:dd-HH_mm_ss}", srcInfo.DateTaken);
        var fileName = Path.GetFileName(srcInfo.FilePath);
        // This is a hack to get around a specific case I ran into where
        // I had copied over some Android pics twice:
        var pattern = @"VID_([0-9]{1,4})([0-9]{1,2})([0-9]{1,2})_([0-9]{1,2})([0-9]{1,2})([0-9]{1,2})([0-9]{1,3})\.mp4";
        var match = Regex.Match(fileName, pattern);
        if (match.Success)
        {
            var i = fileName.IndexOf("VID_");
            fileName = fileName.Substring(i);
        }

        // end hack!

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
        return createFileCopyItem(srcInfo, dstFile);
    }
    
    private FileCopyItem createFileCopyItem(PictureInfo srcInfo, string dstFile)
    {        
        PictureInfo dstInfo = (File.Exists(dstFile)
                            ? new PictureInfo(dstFile)
                            : null);

        FileCopyStatus status = FileCopyStatus.DestinationDoesNotExist;
        if (dstInfo != null)
        {
            if (srcInfo.DateTaken != dstInfo.DateTaken)
            {
                status = FileCopyStatus.DifferentTimes;
            }
            else
            {
                if (srcInfo.Length > dstInfo.Length)
                {
                    status = FileCopyStatus.SourceIsBigger;
                }
                else if (dstInfo.Length > srcInfo.Length)
                {
                    status = FileCopyStatus.DestinationIsBigger;
                }
                else
                {
                    status = FileCopyStatus.SeeminglyIdentical;
                }
            }
        }
       return new FileCopyItem(srcInfo, dstFile, dstInfo, status);
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
