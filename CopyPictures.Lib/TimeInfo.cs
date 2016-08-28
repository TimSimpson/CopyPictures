using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;  // BitmapMetaData

namespace CopyPictures
{

public class Optional<T> 
{  
    public readonly bool IsSet;
    public readonly T Value;

    public Optional(T t)
    {
        Value = t;
        IsSet = true;
    }

    public Optional()
    {        
        IsSet = false;
    }
        
}

public class TimeInfo
{
    public static Optional<DateTime> FindImageFileTime(string filePath)
    {
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open,
                                               FileAccess.Read))
            {
                BitmapSource bs = BitmapFrame.Create(stream);
                BitmapMetadata metadata = (BitmapMetadata) bs.Metadata;
                return new Optional<DateTime>(DateTime.Parse(metadata.DateTaken));
            }
        }
        catch(Exception)
        {
            return new Optional<DateTime>();
        }
    }

    // http://stackoverflow.com/questions/3104641/how-do-i-find-the-date-a-video-avi-mp4-was-actually-recorded
    public static Optional<DateTime> FindAndroidMp4Time(string filePath)
    {
        var pattern = @"VID_([0-9]{1,4})([0-9]{1,2})([0-9]{1,2})_([0-9]{1,2})([0-9]{1,2})([0-9]{1,2})([0-9]{1,3})\.mp4";
        var match = Regex.Match(filePath, pattern);
        if (match.Success)
        {
            try
            {
                int year = Int32.Parse(match.Groups[1].Value);
                int month = Int32.Parse(match.Groups[2].Value);
                int day = Int32.Parse(match.Groups[3].Value);
                int hour = Int32.Parse(match.Groups[4].Value);
                int minute = Int32.Parse(match.Groups[5].Value);
                int seconds = Int32.Parse(match.Groups[6].Value);
                int ms = Int32.Parse(match.Groups[7].Value);

                return new Optional<DateTime>(
                    new DateTime(year, month, day, hour, minute, seconds, ms));
            }
            catch(FormatException)
            {
                return new Optional<DateTime>();
            }
        }
        return new Optional<DateTime>();
   }
}
}
