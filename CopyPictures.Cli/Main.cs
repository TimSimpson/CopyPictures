using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CopyPictures
{
    class Program
    {
        static void Usage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
@"Usage:
    CopyPictures [dupeOption] sourceDirectory destDirectory [dryRun]

Where dupeOption is 'prompt' or 'rename'. If any files would be copied into
the destDirectory with the same name as existing files, if 'prompt' is used
then the program will stop and ask the user whether to copy the file. If
'rename' is used the program will simply copy the file with a slightly
different name.

If 'promptS' is specified, all files will be overwritten automatically if
the source and destination files share the same date taken metadata and the
length of the destination file is less than the source file. This can be useful
in replacing a large number of duplicated pictures with inferior smaller
resolution copies.

If 'dryRun' is specified as the last argument, no files are written to (or
replaced at) the destDirectory but directories will still be created.
    "
            );
        }

        static bool CheckDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("No such directory: " + dir);
                return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            Environment.ExitCode = 1;
            var normal = Console.ForegroundColor;
            if (args.Length < 3)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Incorrect arguments.");                                                       
                Usage();
            }
            else
            {
                var optionStr = args[0];
                var src = args[1];
                var dest = args[2];
                var dryRun = false;
                if (args.Length > 3)
                {
                    if (args[3] == "dryRun")
                    {
                        dryRun = true;
                    }
                    else
                    {
                        Console.Error.WriteLine("Incorrect arguments.");
                        Usage();
                        Console.ForegroundColor = normal;
                        return;
                    }
                }
                PictureDuplicateOptions option = (
                    optionStr == "prompt" ?
                        PictureDuplicateOptions.PromptToOverwrite
                    : (optionStr == "promptS" ?
                            PictureDuplicateOptions.PromptButNotIfTimesMatchAndDestIsSmaller
                        :
                            PictureDuplicateOptions.CopyWithSlightlyDifferentFileName
                        )
                    );


                if (CheckDir(src) && CheckDir(dest))
                {
                    var controller = new ConsoleController();
                    var repo = new PictureRepo(dest, option);                    
                    List<FileCopyItem> files = repo.AddDirectory(src, controller);

                    var copier = new FileCopier(dryRun, option, controller, controller);
                    copier.CopyFiles(files);

                    Environment.ExitCode = 0;
                }
            }
            Console.ForegroundColor = normal;

        }
    }
}
