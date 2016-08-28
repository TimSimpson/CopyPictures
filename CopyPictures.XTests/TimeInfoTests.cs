using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


using CopyPictures;

namespace CopyPictures.XTests
{
    public class TimeInfoTests
    {
        [Fact]
        public void WhenFileNameMatchesTest()
        {
            var time = TimeInfo.FindAndroidMp4Time(@"C:\SomePlace\AnotherPlace\11-19_43_49-VID_20160610_201002853.mp4");
            Assert.True(time.IsSet);
            Assert.Equal(new DateTime(2016, 6, 10, 20, 10, 2, 853), time.Value);
        }

        [Fact]
        public void WhenFileNameDoesNotMatchTest()
        {
            Assert.True(false);
            var time = TimeInfo.FindAndroidMp4Time(@"C:\SomePlace\AnotherPlace\11-19_43_49-VD_20160610_201002853.mp4");
            Assert.False(time.IsSet);
        }
    }
}
