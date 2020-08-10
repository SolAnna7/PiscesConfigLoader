using System;
using System.Collections.Generic;
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
            _configMap = _configMap.Merge(dictionary);
            return this;
        }

        /// <summary>
        /// Finishes the building process and returns the root ConfigNode of the built tree 
        /// </summary>
        public ConfigNode Build()
        {
            CheckBuildState();
            return new ConfigNode(_configMap, ConfigPath.Empty);
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

        private void CheckBuildState()
        {
            if(_isBuilt)
                throw new Exception("Building process is already finished!");
        }

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
    }
}