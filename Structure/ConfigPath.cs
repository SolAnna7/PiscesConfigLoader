using System.Linq;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Structure
{
    public class ConfigPath
    {
        public string[] Path { get; }

        public ConfigPath(params string[] path)
        {
            Path = path;
        }

        public ConfigPath Add(string step) => new ConfigPath(Path.Concat(new[] {step}).ToArray());
        public ConfigPath Add(ConfigPath anotherPath) => new ConfigPath(Path.Concat(anotherPath.Path).ToArray());
        public ConfigPath Add(params string[] steps) => new ConfigPath(Path.Concat(steps).ToArray());

        public static ConfigPath Empty => new ConfigPath();
    }
}