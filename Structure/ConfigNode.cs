using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;
using SnowFlakeGamesAssets.TaurusDungeonGenerator.Utils;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public class ConfigNode : QueryResult
    {
        private readonly IDictionary<object, object> _root;

        public ConfigNode(object value, ConfigPath path) : base(value, path)
        {
            var objects = value as IDictionary<object, object>;
            if (objects != null)
                _root = objects;
        }

        public QueryResult Query(ConfigPath path)
        {
            if (_root == null)
            {
                throw new Exception($"Node has no child elements! (path:{path})");
            }

            return new QueryResult(_root.ReadPath(path.Path), path);
        }

        public QueryResult Query(params string[] path) => Query(new ConfigPath(path));

        public MaybeQueryResult TryQuery(params string[] path) => TryQuery(new ConfigPath(path));

        public MaybeQueryResult TryQuery(ConfigPath path)
        {
            if (_root == null)
            {
                throw new Exception("Node has no child elements!");
            }

            return new MaybeQueryResult(new QueryResult(_root.TryReadPath(path.Path), path));
        }

        public static ConfigNode Empty()
        {
            return new ConfigNode(null, null);
        }

        public IEnumerable<string> GetKeys()
        {
            return _root.Keys.Select(k => k.ToString());
        }

        public ConfigNode UnionWith(ConfigNode other)
        {
            Dictionary<object, object> dict = new Dictionary<object, object>();
            _root.Keys.ForEach(k => dict.Add(k, _root[k]));
            other._root.Keys.ForEach(k => dict.Add(k, other._root[k]));
            return new ConfigNode(dict, new ConfigPath("#UNION#"));
        }
    }
}