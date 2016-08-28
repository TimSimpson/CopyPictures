using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyPictures
{
    public interface Output
    {
        void LogDirectoryItr(string dir);
        void LogFileCopy(FileCopyItem fci, bool skipped);
    }
}
