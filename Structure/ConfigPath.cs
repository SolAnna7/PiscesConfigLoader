using System;
using System.Linq;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    /// <summary>
    /// The object represents a path in a configuration tree
    /// </summary>
    public class ConfigPath
    {
        /// <summary>
        /// The steps of the path
        /// </summary>
        public string[] Path { get; }

        public ConfigPath(params string[] path)
        {
            Path = path;
        }

        /// <summary>
        /// Add a step to the end of the path
        /// </summary>
        public ConfigPath Add(string step) => new ConfigPath(Path.Concat(new[] {step}).ToArray());
        
        /// <summary>
        /// Add another path to the end of the path
        /// </summary>
        public ConfigPath Add(ConfigPath anotherPath) => new ConfigPath(Path.Concat(anotherPath.Path).ToArray());
        
        /// <summary>
        /// Add steps to the end of the path
        /// </summary>
        public ConfigPath Add(params string[] steps) => new ConfigPath(Path.Concat(steps).ToArray());

        /// <summary>
        /// Get a new empty path
        /// </summary>
        public static ConfigPath Empty => new ConfigPath();

        public override string ToString() => string.Join(".",Path);
    }
}