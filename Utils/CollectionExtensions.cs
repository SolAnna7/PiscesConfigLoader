using System;
using System.Collections.Generic;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Utils
{
    public static class CollectionExtensions
    {
        public static IDictionary<object, object> MergeInto(
            this IDictionary<object, object> targetDictionary,
            IDictionary<object, object> sourceDictionary)
        {
            foreach (object key in sourceDictionary.Keys)
            {
                if (!targetDictionary.ContainsKey(key))
                {
                    targetDictionary.Add(key, sourceDictionary[key]);
                }
                else if (targetDictionary[key] is IDictionary<object, object>)
                {
                    targetDictionary[key] = MergeInto((IDictionary<object, object>) targetDictionary[key],
                        (IDictionary<object, object>) sourceDictionary[key]);
                }
                else
                {
                    targetDictionary[key] = sourceDictionary[key];
                }
            }

            return targetDictionary;
        }

        public static object TryReadPath(this IDictionary<object, object> dict, params string[] path)
        {
            ValidatePathArgs(dict, path);
            try
            {
                return dict.ReadPath(path);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static object ReadPath(this IDictionary<object, object> dict, params string[] path)
        {
            ValidatePathArgs(dict, path);

            for (var index = 0; index < path.Length; index++)
            {
                var step = path[index];
                if (dict.ContainsKey(step))
                {
                    var value = dict[step];
                    if (index == path.Length - 1)
                    {
                        return value;
                    }

                    if (value is Dictionary<object, object>)
                    {
                        dict = (Dictionary<object, object>) value;
                    }
                    else
                    {
                        throw new Exception($"Path is dead end! Wrong step: [{step}] in path [{string.Join(".", path)}]");
                    }
                }
                else
                {
                    throw new Exception($"Path step not found! Wrong step: [{step}] in path [{string.Join(".", path)}]");
                }
            }

            throw new Exception("This should not be possible!");
        }

        private static void ValidatePathArgs(IDictionary<object, object> dict, string[] path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));

            if (path.Length < 1)
                throw new ArgumentException("Path should not be empty");
        }
    }
}