using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SnowFlakeGamesAssets.PiscesConfigLoader.Structure;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;
using UnityEngine;
using YamlDotNet.Serialization;

namespace SnowFlakeGamesAssets.PiscesConfigLoader
{
    /// <summary>
    /// A builder class to initialise new ConfigNode trees
    /// </summary>
    public class ConfigBuilder
    {
        private IDictionary<object, object> _configMap = new Dictionary<object, object>();

        private bool _isBuilt = false;
        private IConfigSynchronizationStrategy _synchStrategy;
        private IConfigSynchronizationSerializer _synchSerializer;
        private TextWriter _synchWriter;


        public ConfigBuilder()
        {
        }

        /// <summary>
        /// Parses TextResources on the specified folder or file at path in a Resources folder
        /// The parsed config is merged into the already loaded tree
        /// </summary>
        /// <param name="resourcePath">The path to parse resources at</param>
        /// <param name="parser">The type of parser to use</param>
        /// <returns>This builder instance</returns>
        public ConfigBuilder ParseTextResourceFiles(string resourcePath, ITextConfigParser parser)
        {
            if (resourcePath == null)
                throw new ArgumentNullException(nameof(resourcePath));
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));


            CheckBuildState();
            var configAssets = Resources.LoadAll<TextAsset>(resourcePath);
            ParseAndLoadConfigs(configAssets.Select(x => x.text), parser);

            return this;
        }

        /// <summary>
        /// Parses the given string
        /// The parsed config is merged into the already loaded tree
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <param name="parser">The type of parser to use</param>
        /// <returns>This builder instance</returns>
        public ConfigBuilder ParseString(string str, ITextConfigParser parser)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            CheckBuildState();
            ParseAndLoadConfigs(new[] {str}, parser);
            return this;
        }

        /// <summary>
        /// Merges the given dictionary into the existing config tree
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public ConfigBuilder MergeDictionary(Dictionary<object, object> dictionary)
        {
            CheckBuildState();
            _configMap = _configMap.Merge(dictionary ?? throw new ArgumentNullException(nameof(dictionary)));
            return this;
        }

        public ConfigBuilder SetSynchronization(IConfigSynchronizationStrategy strategy, IConfigSynchronizationSerializer serializer, TextWriter writer)
        {
            _synchWriter = writer ?? throw new ArgumentNullException(nameof(writer));
            _synchSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _synchStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

            return this;
        }

        /// <summary>
        /// Finishes the building process and returns the root ConfigNode of the built tree 
        /// </summary>
        public ConfigNode Build()
        {
            ValidateBeforeBuild(false);
            return new ConfigNode(_configMap, ConfigPath.Empty);
        }

        public MutableConfigNode BuildMutable()
        {
            ValidateBeforeBuild(true);
            var mutableConfigNode = new MutableConfigNode(_configMap, ConfigPath.Empty, null);
            SetupSynchronization(mutableConfigNode);
            return mutableConfigNode;
        }

        private void SetupSynchronization(MutableConfigNode mutableConfigNode)
        {
            if (_synchSerializer == null)
                return;
            _synchStrategy.Init(() => _synchSerializer.Serialize(_configMap, _synchWriter));
            mutableConfigNode.ValueChanged += () => _synchStrategy.DataChanged();
        }

        private void ParseAndLoadConfigs(IEnumerable<string> contents, ITextConfigParser parser)
        {
            contents
                .Select(parser.ParseText)
                .OrderByDescending(x =>
                {
                    if (x.TryReadPath("meta", "priority") is int priority)
                        return priority;
                    return 0;
                }).ToList().ForEach(map => _configMap = _configMap.Merge(map));
        }

        private void ValidateBeforeBuild(bool isMutable)
        {
            if (_synchSerializer != null && !isMutable)
            {
                throw new Exception("Config synchronisation is set, but config is not mutable");
            }

            CheckBuildState();
        }

        private void CheckBuildState()
        {
            if (_isBuilt)
                throw new Exception("Building process is already finished!");
        }


        #region Util classes

        public interface ITextConfigParser
        {
            Dictionary<object, object> ParseText(string text);
        }

        public class YamlTextConfigParser : ITextConfigParser
        {
            public YamlTextConfigParser()
            {
            }

            public YamlTextConfigParser(Deserializer deserializer) => _deserializer = deserializer;

            private readonly Deserializer _deserializer = new Deserializer();

            public Dictionary<object, object> ParseText(string text) => _deserializer.Deserialize<Dictionary<object, object>>(text);
        }

        #endregion
    }
}