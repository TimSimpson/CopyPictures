Copy Pictures
=============

This is a simple program I use to organize my pictures.

Everyone always wonders what criteria they should use to place and locate
pictures on their hard drive. Sometimes they might put pictures in a directory
named "Pets" or "Funny". Kind of like tagging, but you only get one.

Everyone's opinion differs, but in my mind the only real option is to store
them based on the date they were taken. Everything else is subject to change.

Usage:
    CopyPictures.exe srcDirectory destinationDirectory

This program will thus copy pictures from one directory into another one,
giving each file a name and directory based on the date. So if a picture is
named "NewCar.jpg" and was taken on 2011-03-27 at 2:38 PM, the file name
will be "2011/03/27-02_28_37-NewCar.jpg" with the directories 2011 and 03
being created in whatever destination directory was specified on the command
line.

It determines the date taken by first trying to access the "date taken"
metadata which most digital cameras will store inside the picture itself. If
this fails for whatever reason, it will use the file modified times.


Building
========

You'll need Windows 8 and .NET 4. Sorry. No Visual Studio necessary, just
run "msbuild CopyPictures.csproj".


Caveats / Warnings
==================

This program comes with NO WARRANTIES or liability of any kind. Run at your
own risk! While it should do nothing more than read the original pictures, you
would be very wise to investigate the results of a copy rather than just trust
everything worked. They are your memories after all.
