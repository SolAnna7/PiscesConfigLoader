using System.Collections.Generic;
using GuildTest.Scripts.Config;
using NUnit.Framework;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;

namespace _Tests.Editor
{
    public class DictionaryMergeTest
    {
        [Test]
        public void TestDictionaryMerge1()
        {
            Dictionary<object, object> testDict1 = new Dictionary<object, object>
            {
                {"111", "aaa"}
            };

            Dictionary<object, object> testDict2 = new Dictionary<object, object>
            {
                {"222", "bbb"}
            };

            testDict1.MergeInto(testDict2);

            Assert.True(testDict1.Count == 2);
        }

        [Test]
        public void TestDictionaryMerge2()
        {
            Dictionary<object, object> testDict1 = new Dictionary<object, object>
            {
                {"111", "aaa"}
            };

            Dictionary<object, object> testDict2 = new Dictionary<object, object>
            {
                {"111", "bbb"}
            };

            testDict1.MergeInto(testDict2);

            Assert.AreEqual(1, testDict1.Count);
            Assert.True(testDict1.ContainsKey("111"));
            Assert.AreEqual("bbb", testDict1["111"]);
        }

        [Test]
        public void TestDictionaryMerge3()
        {
            Dictionary<object, object> testDict1 = new Dictionary<object, object>
            {
                {"111", "aaa"},
                {
                    "222", new Dictionary<object, object>
                    {
                        {"999", "xxx"}
                    }
                }
            };

            Dictionary<object, object> testDict2 = new Dictionary<object, object>
            {
                {"111", "bbb"},
                {
                    "333", new Dictionary<object, object>
                    {
                        {"888", "yyy"}
                    }
                }
            };

            testDict1.MergeInto(testDict2);

            Assert.AreEqual(3, testDict1.Count);

            Assert.AreEqual("bbb", testDict1["111"]);

            Assert.AreEqual("xxx", ((Dictionary<object, object>) testDict1["222"])["999"]);
            Assert.AreEqual("yyy", ((Dictionary<object, object>) testDict1["333"])["888"]);
        }

        [Test]
        public void TestDictionaryMerge4()
        {
            Dictionary<object, object> testDict1 = new Dictionary<object, object>
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

            Dictionary<object, object> testDict2 = new Dictionary<object, object>
            {
                {"111", "bbb"},
                {
                    "222", new Dictionary<object, object>
                    {
                        {"888", "yyy"},
                        {"777", "qqq"}
                    }
                }
            };

            testDict1.MergeInto(testDict2);

            Assert.AreEqual(2, testDict1.Count);

            Assert.AreEqual("bbb", testDict1["111"]);

            Assert.AreEqual("xxx", ((Dictionary<object, object>) testDict1["222"])["999"]);
            Assert.AreEqual("yyy", ((Dictionary<object, object>) testDict1["222"])["888"]);
            Assert.AreEqual("qqq", ((Dictionary<object, object>) testDict1["222"])["777"]);
        }
    }
}