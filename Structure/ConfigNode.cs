using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SnowFlakeGamesAssets.PiscesConfigLoader.Utils;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    /// <summary>
    /// Node element of the config tree structure
    /// </summary>
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public class ConfigNode
    {
        private readonly IDictionary<object, object> _root;
        private readonly ConfigPath _path;

        internal ConfigNode(IDictionary<object, object> value, ConfigPath path)
        {
            _root = value ?? throw new ArgumentNullException(nameof(value));
            _path = path;
        }

        /// <summary>
        /// Returns the value specified by the path relative to this node
        /// </summary>
        /// <param name="path">The relative path to the required value</param>
        /// <returns>The value (of unknown type) if the path is valid</returns>
        /// <exception cref="PathReadingException">If the path is not valid</exception>
        /// <exception cref="LeafNodeException">If the node has ne children</exception>
        public QueryResult Query(ConfigPath path) => new QueryResult(_root.ReadPath(path.Path), path);

        /// <summary>
        /// Returns the value specified by the path relative to this node
        /// </summary>
        /// <param name="path">The relative path to the required value</param>
        /// <returns>The value (of unknown type) if the path is valid</returns>
        /// <exception cref="PathReadingException">If the path is not valid</exception>
        public QueryResult Query(params string[] path) => Query(new ConfigPath(path));

        /// <summary>
        /// Returns a maybe value specified by the path relative to this node
        /// </summary>
        /// <param name="path"></param>
        public MaybeQueryResult TryQuery(params string[] path) => TryQuery(new ConfigPath(path));

        /// <summary>
        /// Returns a maybe value specified by the path relative to this node
        /// </summary>
        /// <param name="path"></param>
        public MaybeQueryResult TryQuery(ConfigPath path) => new MaybeQueryResult(_root.TryReadPath(path.Path), path);

        /// <summary>
        /// Returns the keys of the children of this node
        /// </summary>
        public IEnumerable<string> GetKeys() => _root.Keys.Select(k => k.ToString());
    }
}