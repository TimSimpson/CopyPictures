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
        /// Given another PictureInfo (another file) prints out details. This
        /// is used to prompt the user to see if they want to overwrite one
        /// file or the other.
        /// </summary>
        /// <param name="other">The other file.</param>
        public void PrintVerboseInfo(PictureInfo other)
        {
            bool bigger = other != null && (this.Length > other.Length);

            Console.WriteLine("\t" + this.FilePath);
            var modTime = File.GetLastWriteTime(this.FilePath);
            var length = new FileInfo(this.FilePath).Length;
            Console.Write("\t\tSize: " + this.Length);
            if (bigger)
            {
                Console.Write("\t\t<-- BIGGER!!!");
            }
            Console.WriteLine();
            Console.WriteLine("\t\tDate Taken    : {0:yyy-MM-dd HH:mm:ss}",
                              this.DateTaken);
            Console.WriteLine("\t\tFile Mode Time: {0:yyy-MM-dd HH:mm:ss}",
                             this.FileModTime);
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
