using System;
using System.IO;

namespace CopyPictures
{
    /// <summary>
    /// Represents a file. Has properties for the size and the date taken to
    /// make sorting this stuff easier.
    /// </summary>
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

            Optional<DateTime> result = new Optional<DateTime>();
            result = TimeInfo.FindImageFileTime(filePath);
            if (!result.IsSet)
            {
                result = TimeInfo.FindAndroidMp4Time(filePath);
            }

            if (result.IsSet)
            {
                this.DateTaken = result.Value;
            }
            else
            {
                this.DateTaken = this.FileModTime;
            }
        }

        

        /// <summary>
        /// The heart of the program. Compares to see if one picture is the
        /// same as another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsProbablyTheSameAs(PictureInfo other)
        {
            return other.DateTaken == this.DateTaken
                && other.Length == this.Length;
        }

    }
}
