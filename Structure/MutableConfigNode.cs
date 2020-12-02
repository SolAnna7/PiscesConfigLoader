using System;
using System.Collections.Generic;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    public class MutableConfigNode : ConfigNode
    {
        internal MutableConfigNode(IDictionary<object, object> value, ConfigPath path, MutableConfigNode parent) : base(value, path)
        {
            if (parent != null)
                ValueChanged += () => parent.ValueChanged.Invoke();
        }

        public event Action ValueChanged;

        public void AddValue(string key, object value)
        {
            if (_root.ContainsKey(key))
                throw new Exception($"Key [{key}] is already exists in mutable config node");
            _root.Add(key, value);
            ValueChanged?.Invoke();
        }

        public void SetValue(string key, object value)
        {
            if (!_root.ContainsKey(key))
                throw new Exception($"Key [{key}] does not exists in mutable config node");
            _root[key] = value;
            ValueChanged?.Invoke();
        }
    }
}