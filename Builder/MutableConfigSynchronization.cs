using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SnowFlakeGamesAssets.PiscesConfigLoader
{
    #region Interfaces

    public interface IConfigSynchronizationStrategy
    {
        void Init(Action callback);

        void DataChanged();

        bool IsDirty { get; }
    }

    public interface IConfigSynchronizationSerializer
    {
        void Serialize(IDictionary<object, object> data, TextWriter writer);
    }

    public interface IConfigSynchronizationTarget
    {
        TextWriter GetWriter();
    }

    #endregion

    #region Implementations

    public class OuterCommandSynchronizationStrategy : IConfigSynchronizationStrategy
    {
        private Action _callback;

        public void Init(Action callback)
        {
            _callback = callback;
        }

        public void DataChanged() => IsDirty = true;

        public bool IsDirty { get; private set; } = false;

        public bool SynchronizeIfDirty()
        {
            if (!IsDirty)
                return false;

            _callback.Invoke();

            IsDirty = false;
            return true;
        }
    }


    public class YamlSerializer : IConfigSynchronizationSerializer
    {
        public void Serialize(IDictionary<object, object> data, TextWriter writer)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(new HyphenatedNamingConvention()).Build();
            serializer.Serialize(writer, data);
        }
    }
    
    public class ConfigFileWriter : IConfigSynchronizationTarget
    {
        private readonly string _outputFilePath;

        public ConfigFileWriter(string outputFilePath)
        {
            _outputFilePath = outputFilePath;
        }

        public TextWriter GetWriter() => new StreamWriter(_outputFilePath);
    }
    
    #endregion
}