using System;
using System.Collections.Generic;
using NUnit.Framework;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Tests
{
    public class PathReadingTest
    {
        [Test]
        public void TestPathReading()
        {
            Dictionary<object, object> testDict = new Dictionary<object, object>
            {
                {"111", "aaa"},
                {
                    "222", new Dictionary<object, object>
                    {
                        {"999", "xxx"},
                        {"777", "zzz"}
                    }
                }
            };


            try
            {
                testDict.ReadPath("000");
                Assert.Fail("This element does not exists!");
            }
            catch (Exception)
            {
                // ignored
            }

            Assert.AreEqual("aaa", testDict.ReadPath("111"));

            Assert.AreEqual("xxx", testDict.ReadPath("222", "999"));
            Assert.AreEqual("zzz", testDict.ReadPath("222", "777"));

            try
            {
                testDict.ReadPath("222", "999", "xyz");
                Assert.Fail("This element does not exists!");
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}