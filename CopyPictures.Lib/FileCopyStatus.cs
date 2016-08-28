using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyPictures
{
/// <summary>
/// Represents desired outcome of file copy operation.
/// </summary>
public enum FileCopyStatus
{
    DestinationDoesNotExist,
    DestinationIsBigger,
    DifferentTimes,
    SeeminglyIdentical,    
    SourceIsBigger
}
}
