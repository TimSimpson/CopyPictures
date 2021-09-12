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


    public static Optional<DateTime> FindAndroidMp4Time(string filePath)
    {
        var pattern = @"VID_([0-9]{1,4})([0-9]{1,2})([0-9]{1,2})_([0-9]{1,2})([0-9]{1,2})([0-9]{1,2})([0-9]{1,3})\.mp4";
        var result =  findAndroidMp4TimeWithPattern(pattern, filePath);
        if (!result.IsSet) {
            // In 2021 I got a Pixel phone and found it uses this file name.
            // The hours are a bit off but the day is correct. If I ever truly
            // am bothered I could always add a DLL I read about to look
            // at the "Media Created" attribute.
            pattern = @"PXL_([0-9]{1,4})([0-9]{1,2})([0-9]{1,2})_([0-9]{1,2})([0-9]{1,2})([0-9]{1,2})([0-9]{1,3})\.mp4";
            result =  findAndroidMp4TimeWithPattern(pattern, filePath);
        }
        return result;
    }

    // http://stackoverflow.com/questions/3104641/how-do-i-find-the-date-a-video-avi-mp4-was-actually-recorded
    public static Optional<DateTime> findAndroidMp4TimeWithPattern(string pattern, string filePath)
    {
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
                // 2019-01-05: Get ready for another kludge straight from the 
                // bowels of hell. I know think the filename in Android matches
                // as far as the day is concerned but the time of day has nothing
                // to do with it.
                // Specifically, there was a file named 
                // "VID_20181130_000118147.mp4" which lead to hours = -1.
                // Looking at the properties in Windows explorer though the "Media
                // created" time (the only extended property with a timestamp I
                // could find) was 2018-11-30 2:01 AM. That doesn't map in any
                // way to the file name. SO, this is seemingly my only choice.
                // Thankfully I've found that, in practice, we never care about
                // exact time of day matching when we browse photos.
                // It does really bother me though...
                if (hour < 0)
                {
                    hour = 1;
                }
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
