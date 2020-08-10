using System.Collections.Generic;
using NUnit.Framework;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Tests
{
    public class DictionaryMergeTest
    {
        [Test]
        public void TestDictionaryMerge1()
        {
            Dictionary<object, object> testDict1 = new Dictionary<object, object>
            {
                {"111", "aaa"},
            };

            Dictionary<object, object> testDict2 = new Dictionary<object, object>
            {
                {"222", "bbb"},
                {"333", "ccc"}
            };

            var res = testDict1.Merge(testDict2);

            Assert.True(res.Count == 3);
            Assert.True(testDict1.Count == 1);
            Assert.True(testDict2.Count == 2);
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

            var res = testDict1.Merge(testDict2);

            Assert.AreEqual(1, res.Count);
            Assert.True(res.ContainsKey("111"));
            Assert.AreEqual("bbb", res["111"]);
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

            var res = testDict1.Merge(testDict2);

            Assert.AreEqual(3, res.Count);

            Assert.AreEqual("bbb", res["111"]);

            Assert.AreEqual("xxx", ((Dictionary<object, object>) res["222"])["999"]);
            Assert.AreEqual("yyy", ((Dictionary<object, object>) res["333"])["888"]);
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

            var res = testDict1.Merge(testDict2);

            Assert.AreEqual(2, res.Count);

            Assert.AreEqual("bbb", res["111"]);

            Assert.AreEqual("xxx", ((Dictionary<object, object>) res["222"])["999"]);
            Assert.AreEqual("yyy", ((Dictionary<object, object>) res["222"])["888"]);
            Assert.AreEqual("qqq", ((Dictionary<object, object>) res["222"])["777"]);
        }
    }
}