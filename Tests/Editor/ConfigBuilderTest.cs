using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Tests
{
    public class ConfigBuilderTest
    {
        [Test]
        public void TestEmptyBuilding()
        {
            var configRoot = new ConfigBuilder().Build();
            Assert.AreEqual(0, configRoot.GetKeys().Count());
        }

        [Test]
        public void TestStringToYamlParsing()
        {
            var configRoot =
                new ConfigBuilder()
                    .ParseString(TestYamlStr, new ConfigBuilder.YamlTextConfigParser())
                    .Build();
            Assert.AreEqual(1, configRoot.GetKeys().Count());
            Assert.AreEqual(123, configRoot.Query("aaa", "bbb").AsInt());
            Assert.AreEqual("ddd", configRoot.Query("aaa", "ccc").AsString());
        }

        [Test]
        public void TestResourceFileToYamlParsing()
        {
            var configRoot =
                new ConfigBuilder()
                    .ParseTextResourceFiles("TestYamlFiles/TestYaml1", new ConfigBuilder.YamlTextConfigParser())
                    .Build();
            Assert.AreEqual(1, configRoot.GetKeys().Count());
            Assert.AreEqual(987, configRoot.Query("xxx", "yyy").AsInt());
            Assert.AreEqual("www", configRoot.Query("xxx", "zzz").AsString());
        }

        [Test]
        public void TestResourceDirectoryFilesToYamlParsing()
        {
            var configRoot =
                new ConfigBuilder()
                    .ParseTextResourceFiles("TestYamlFiles", new ConfigBuilder.YamlTextConfigParser())
                    .Build();
            Assert.AreEqual(2, configRoot.GetKeys().Count());
            Assert.AreEqual(987, configRoot.Query("xxx", "yyy").AsInt());
            Assert.AreEqual("www", configRoot.Query("xxx", "zzz").AsString());
            Assert.AreEqual(919, configRoot.Query("xxx", "www").AsInt());
            Assert.AreEqual(456, configRoot.Query("jjj", "kkk").AsInt());
            Assert.AreEqual("ddd", configRoot.Query("jjj", "lll").AsString());
        }

        [Test]
        public void TestMergingDictionaryToConfig()
        {
            var configRoot =
                new ConfigBuilder()
                    .MergeDictionary(
                        new Dictionary<object, object>
                        {
                            {
                                "aaa", new Dictionary<object, object>
                                {
                                    {"bbb", 999},
                                }
                            }
                        }
                    )
                    .MergeDictionary(
                        new Dictionary<object, object>
                        {
                            {
                                "aaa", new Dictionary<object, object>
                                {
                                    {"ccc", 111},
                                }
                            },
                            {
                                "xxx", 555
                            }
                        }
                    ).Build();
            Assert.AreEqual(2, configRoot.GetKeys().Count());
            Assert.AreEqual(999, configRoot.Query("aaa", "bbb").AsInt());
            Assert.AreEqual(111, configRoot.Query("aaa", "ccc").AsInt());
            Assert.AreEqual(555, configRoot.Query("xxx").AsInt());
        }

        [Test]
        public void TestCustomConfigParser()
        {
            var config1 =
                new ConfigBuilder()
                    .ParseString("maybe?", new TestConfigParser())
                    .Build();
            var config2 =
                new ConfigBuilder()
                    .ParseString("please!", new TestConfigParser())
                    .Build();

            Assert.AreEqual(1, config1.GetKeys().Count());
            Assert.AreEqual("sorry", config1.Query("nope").AsString());

            Assert.AreEqual(1, config2.GetKeys().Count());
            Assert.AreEqual(000, config2.Query("yesssss", "oh_yeah").AsInt());
        }

        private class TestConfigParser : ConfigBuilder.ITextConfigParser
        {
            public Dictionary<object, object> ParseText(string text)
            {
                if (text == "please!")
                    return new Dictionary<object, object>
                    {
                        {
                            "yesssss", new Dictionary<object, object>
                            {
                                {"oh_yeah", 000},
                            }
                        }
                    };

                return new Dictionary<object, object>
                {
                    {"nope", "sorry"}
                };
            }
        }

        private const string TestYamlStr = @"
aaa:
  bbb: 123
  ccc: ddd
";
    }
}