using System;
using System.IO;
using NUnit.Framework;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Tests
{
    public class MutableConfigTest
    {
        [Test]
        public void ChangeTest()
        {
            var config = new ConfigBuilder()
                .ParseTextResourceFiles("TestYamlFiles/MutableTest", new ConfigBuilder.YamlTextConfigParser())
                // .SetSynchronization(outerCommandSynchronizationStrategy, new YamlSerializer(), new ConfigFileWriter("Settings/settings"))
                .BuildMutable();

            Assert.AreEqual(123, config.Query("aaa", "bbb").AsInt());

            config.AddValue("eee", "yay");

            Assert.AreEqual("yay", config.Query("eee").AsString());

            config.SetValue("eee", "yay2");
            Assert.AreEqual("yay2", config.Query("eee").AsString());

            try
            {
                config.AddValue("eee", "nooo");
                Assert.Fail();
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                config.SetValue("www", "???");
                Assert.Fail();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [Test]
        public void SynchTest()
        {
            var outerCommandSynchronizationStrategy = new OuterCommandSynchronizationStrategy();
            StringWriter writer = new StringWriter();
            var config = new ConfigBuilder()
                .ParseTextResourceFiles("TestYamlFiles/MutableTest", new ConfigBuilder.YamlTextConfigParser())
                .SetSynchronization(outerCommandSynchronizationStrategy, new YamlSerializer(), writer)
                .BuildMutable();

            config.AddValue("eee", "yay");
            Assert.IsTrue(outerCommandSynchronizationStrategy.IsDirty);
            Assert.IsTrue(outerCommandSynchronizationStrategy.SynchronizeIfDirty());
            Assert.IsFalse(outerCommandSynchronizationStrategy.IsDirty);

            var str = writer.ToString();

            StringReader reader = new StringReader(str);

            Assert.AreEqual("aaa:", reader.ReadLine());
            Assert.AreEqual("  bbb: 123", reader.ReadLine());
            Assert.AreEqual("eee: yay", reader.ReadLine());
        }
    }
}