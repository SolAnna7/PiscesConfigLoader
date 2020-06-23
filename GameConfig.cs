using System;
using System.Collections.Generic;
using System.Linq;
using SnowFlakeGamesAssets.PiscesConfigLoader.Component;
using SnowFlakeGamesAssets.PiscesConfigLoader.Structure;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils.Expression;
using UnityEngine;
using Exception = System.Exception;
using Random = UnityEngine.Random;

namespace SnowFlakeGamesAssets.PiscesConfigLoader
{
    public static class GameConfig
    {
        private static IDictionary<object, object> _configMap = new Dictionary<object, object>();

        public static ConfigNode Root => new ConfigNode(_configMap, ConfigPath.Empty);

        private static readonly List<IConfigReader> Readers = new List<IConfigReader>();

        public static ExpressionParser ExpressionParser { get; } = new ExpressionParser();    
        
        public static void InitConfig()
        {
            if (_configMap.Count != 0)
                throw new Exception("Config already initialized!");

            if (Readers.Count == 0)
                throw new Exception("No readers registered!");

            foreach (var reader in Readers.OrderBy(x => x.GetPriority()).Reverse())
            {
                Debug.Log($"Loading config from path {reader.GetConfigPath()}");
                var configAssets = reader.LoadConfigAssets();
                ParseAndLoadConfigs(configAssets.Select(x => x.text));
            }
            
            ExpressionParser.AddFunc("rand", RangedRandomFunction);
        }

        private static double RangedRandomFunction(double[] inputs)
        {
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            if (inputs.Length != 2) throw new ArgumentException("Function random should have only two input parameter.", nameof(inputs));

            float min = (float) inputs[0];
            float max = (float) inputs[1];

            return Random.Range(min, max);
        }

        public static void Clear() => _configMap = new Dictionary<object, object>();

        public static void ReinitConfig()
        {
            Clear();
            InitConfig();
        }

        public static bool IsInitialized => _configMap.Count != 0;

        public static void RegisterReader(IConfigReader reader) => Readers.Add(reader);

        public static void ParseAndLoadConfig(string content)
        {
            var deserializer = new YamlDotNet.Serialization.Deserializer();
            _configMap = _configMap.MergeInto(deserializer.Deserialize<Dictionary<object, object>>(content));
        }

        public static void ParseAndLoadConfigs(IEnumerable<string> contents)
        {
            var deserializer = new YamlDotNet.Serialization.Deserializer();

            var maps = contents
                .Select(content => deserializer.Deserialize<Dictionary<object, object>>(content))
                .OrderByDescending(x =>
                {
                    var priority = x.TryReadPath("meta", "priority");
                    if (priority is int)
                        return (int) priority;
                    return 0;
                });

            foreach (var map in maps)
            {
                _configMap = _configMap.MergeInto(map);
            }
        }

        public static QueryResult Query(ConfigPath path)
        {
            if (_configMap.Count == 0)
                throw new Exception("Config not yet initialized!");

            return Root.Query(path);
        }

        public static QueryResult Query(params string[] path)
        {
            if (_configMap.Count == 0)
                throw new Exception("Config not yet initialized!");

            return Query(new ConfigPath(path));
        }

        public static MaybeQueryResult TryQuery(ConfigPath path) => Root.TryQuery(path);

        public static MaybeQueryResult TryQuery(params string[] path) => Root.TryQuery(path);
    }
}