using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;  // BitmapMetaData

namespace CopyPictures
{

public class Optional<T>
{
    public readonly bool IsSet;
    private readonly T v;

    public Optional(T t)
    {
        v = t;
        IsSet = true;
    }

    public Optional()
    {
        IsSet = false;
    }

    public T Value
    {
        get
        {
            if (IsSet)
            {
                return v;
            }
            throw new Exception("Optional value not set!");
        }
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
                int hour = Int32.Parse(match.Groups[4].Value) - 1;
                /*if (hour >= 24)
                {
                    // No, I have no idea WTF this is necessary.
                    // Basically I had a ton of pictures which ended up almost
                    // being duplicated; I'd copied some using the MTP mode
                    // (USB to Android phone) so the created dates where 
                    // preserved, but then I started using the Boar VCS, 
                    // which apparently doesn't give a shit about the created
                    // date. So at that point I had dozens of duplicates.
                    // The only way they synced is if I set the hour to what
                    // appeared in the file name- but the problem then was
                    // one file was set to 24.
                    hour = 24;
                }*/
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
