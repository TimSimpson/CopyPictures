using System;
using System.Collections.Generic;
using System.IO;

namespace CopyPictures
{
public class FileCopier
{
    private readonly PictureDuplicateOptions dupeOptions;
    private readonly bool dryRun;    
    private ConsoleController controller;
    private Output output;

    public FileCopier(bool dryRun, PictureDuplicateOptions dupeOptions,
                      ConsoleController controller,
                      Output output)
    {
        this.dryRun = dryRun;
        this.dupeOptions = dupeOptions;
        this.controller = controller;
        this.output = output;
    }

    private bool chooseToOverwrite(PictureInfo src, PictureInfo dst)
    {
        return controller.ChooseToOverwrite(src, dst);       
    }
    
    public void CopyFiles(List<FileCopyItem> items)
    {
        foreach (FileCopyItem item in items)
        {
            CopyFile(item);
        }
    }

    /// <summary>
    /// Actually copies a picture to a destination file. May prompt if 
    /// needed.
    /// </summary>
    /// <param name="fci">Info on what to copy.</param>
    private void CopyFile(FileCopyItem fci)
    {   
        bool overwrite = false;
        if (fci.Dst != null)
        {
            if (this.dupeOptions != PictureDuplicateOptions.PromptToOverwrite
                && this.dupeOptions != PictureDuplicateOptions.PromptButNotIfTimesMatchAndDestIsSmaller)
            {
                throw new Exception("Bug!");
            }
            bool summonPrompt = true;
            if (dupeOptions == PictureDuplicateOptions.PromptButNotIfTimesMatchAndDestIsSmaller)
            {
                if (fci.Status == FileCopyStatus.DifferentTimes)
                {
                    // Always prompt if the times are different.
                    summonPrompt = true;
                }
                else
                {
                    // If the source file is larger, prompt to overwrite.
                    // This is useful if a new source of pictures is found that
                    // were taken at a higher resolution than the old ones.
                    if (fci.Status == FileCopyStatus.SourceIsBigger)
                    {
                        overwrite = true;
                        summonPrompt = true;
                    }
                    // Don't overwrite if the destination file is larger.
                    else if (fci.Status == FileCopyStatus.SeeminglyIdentical
                             || fci.Status == FileCopyStatus.DestinationIsBigger)
                    {
                        overwrite = false;
                        summonPrompt = false;
                    }
                }
            }
            if (summonPrompt)
            {
                overwrite = controller.ChooseToOverwrite(fci.Source, fci.Dst);
            }
            if (!overwrite)
            {                
                output.LogFileCopy(fci, true);
                return;
            }
            else
            {
                overwrite = true;
            }
        }

        output.LogFileCopy(fci, false);
        if (dryRun == true)
        {
            return;
        }
        File.Copy(fci.Source.FilePath, fci.DstFilePath, overwrite);
        File.SetLastWriteTime(fci.DstFilePath, fci.Source.DateTaken);
    }

}
}
