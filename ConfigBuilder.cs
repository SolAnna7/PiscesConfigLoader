using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SnowFlakeGamesAssets.PiscesConfigLoader.Structure;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils.Expression;
using SnowFlakeGamesAssets.TaurusDungeonGenerator.Utils;
using UnityEngine;
using YamlDotNet.Serialization;

namespace SnowFlakeGamesAssets.PiscesConfigLoader
{
    public class ConfigBuilder
    {
        private static IDictionary<object, object> _configMap = new Dictionary<object, object>();

        public ConfigBuilder()
        {
        }

        public ConfigBuilder ParseTextResourceFiles(string resourcePath, ITextConfigParser parser)
        {
            var configAssets = Resources.LoadAll<TextAsset>(resourcePath);
            ParseAndLoadConfigs(configAssets.Select(x => x.text), parser);

            return this;
        }

        public ConfigBuilder ParseString(string str, ITextConfigParser parser)
        {
            ParseAndLoadConfigs(new[] {str}, parser);
            return this;
        }

        public ConfigNode Build()
        {
            //todo: expression parser
            // ExpressionParser.AddFunc("rand", RangedRandomFunction);
            return new ConfigNode(_configMap, ConfigPath.Empty);
        }

        private static void ParseAndLoadConfigs(IEnumerable<string> contents, ITextConfigParser parser)
        {
            contents
                .Select(parser.ParseText)
                .OrderByDescending(x =>
                {
                    if (x.TryReadPath("meta", "priority") is int priority)
                        return priority;
                    return 0;
                }).ForEach(map => _configMap.MergeInto(map));
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