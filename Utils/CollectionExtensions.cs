using System;
using System.Collections.Generic;
using SnowFlakeGamesAssets.PiscesConfigLoader.Structure;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Utils
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Creates a new dictionary with the merge of the two input
        /// If the same key exists in both, then if the values of the keys are dictionaries, than they are merged.
        /// Otherwise the target value is overwritten.
        /// </summary>
        public static IDictionary<object, object> Merge(this IDictionary<object, object> targetDictionary, IDictionary<object, object> sourceDictionary)
        {
            Dictionary<object, object> copy = new Dictionary<object, object>();
            foreach (var targetDictionaryKey in targetDictionary.Keys)
                copy[targetDictionaryKey] = targetDictionary[targetDictionaryKey];
            
            foreach (object key in sourceDictionary.Keys)
            {
                if (!copy.ContainsKey(key))
                {
                    copy.Add(key, sourceDictionary[key]);
                }
                else if (copy[key] is IDictionary<object, object>)
                {
                    copy[key] = Merge((IDictionary<object, object>) copy[key], (IDictionary<object, object>) sourceDictionary[key]);
                }
                else
                {
                    copy[key] = sourceDictionary[key];
                }
            }
            
            return copy;
        }

        /// <summary>
        /// If the path is valid in the dictionary tree returns the value in the path, else null 
        /// </summary>
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

        /// <summary>
        /// If the path is valid in the dictionary tree returns the value in the path, else throws an exception 
        /// </summary>
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
                        throw new PathReadingException($"Path is dead end! Wrong step: [{step}] in path [{string.Join(".", path)}]");
                    }
                }
                else
                {
                    throw new PathReadingException($"Path step not found! Wrong step: [{step}] in path [{string.Join(".", path)}]");
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