using System.IO;

namespace CopyPictures
{
    /// <summary>
    /// An order to copy a file.
    /// </summary>
    public class FileCopyItem
    {
        public readonly PictureInfo Dst;
        public readonly string DstFilePath;
        public readonly PictureInfo Source;
        public readonly FileCopyStatus Status;

        public FileCopyItem(PictureInfo srcInfo, string dstFile,
                            PictureInfo dstInfo, FileCopyStatus status)
        {
            this.DstFilePath = dstFile;
            this.Source = srcInfo;
            this.Dst = dstInfo;
            this.DstFilePath = dstFile;
            this.Status = status;
        }
    }
}
