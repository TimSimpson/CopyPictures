using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;


using CopyPictures;

namespace CopyPictures.XTests
{
    public class TimeInfoTests
    {
        [Fact]
        public void WhenFileNameMatchesAndroidMp4Pattern()
        {
            var time = TimeInfo.FindAndroidMp4Time(@"C:\SomePlace\AnotherPlace\11-19_43_49-VID_20160610_201002853.mp4");
            Assert.True(time.IsSet);
            Assert.Equal(new DateTime(2016, 6, 10, 19, 10, 2, 853), time.Value);
        }

        [Fact]
        public void WhenFileNameDoesNotMatchAndroidMp4Pattern()
        {
            var time = TimeInfo.FindAndroidMp4Time(@"C:\SomePlace\AnotherPlace\11-19_43_49-VD_20160610_201002853.mp4");
            Assert.False(time.IsSet);
        }

        [Fact]
        public void TimeIsDiscoveredFromImageAttributes()
        {
            var path = Directory.GetCurrentDirectory() + @"..\..\..\dino.jpg";
            Assert.True(File.Exists(path));
            var time = TimeInfo.FindImageFileTime(path);
            Assert.True(time.IsSet);
            Assert.Equal(new DateTime(2015, 10, 3, 16, 33, 16, 0), time.Value);
        }
    }
}
