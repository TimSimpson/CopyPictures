#tool "xunit.runner.console"
#tool "ReportUnit"

var report = Directory("./reports");
var xunitReport = report + Directory("xunit");

var solution = File("./CopyPictures.sln");

Task("build").Does(() =>
{
  MSBuild(solution);
});

Task("test")
  .IsDependentOn("build")
  .Does(() =>
  {
    CreateDirectory(xunitReport);
    CleanDirectories(xunitReport);

    var filePath = "./CopyPictures.XTests/bin/Debug/CopyPictures.XTests.dll";
    if (!FileExists(filePath))
    {
        throw new Exception("MISSING TEST DLL!");
    }
    XUnit2(filePath, new XUnit2Settings
    {
      XmlReport = true,
      OutputDirectory = xunitReport
    });
  }).Finally(() =>
  {
    ReportUnit(xunitReport);
  });

var target = Argument("target", "default");
var configuration = Argument("configuration", "release");

Task("default")
  .IsDependentOn("test");

RunTarget(target);
