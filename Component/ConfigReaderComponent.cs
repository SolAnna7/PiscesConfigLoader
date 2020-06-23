using System.Collections.Generic;
using UnityEngine;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Component
{
    public class ConfigReaderComponent : MonoBehaviour, IConfigReader
    {
        public int Priority = 0;
        private static string ConfigRootPathBaseValue => "Config";
        public string ConfigPath = ConfigRootPathBaseValue;

        private void Reset()
        {
            ConfigPath = ConfigRootPathBaseValue;
            Priority = 0;
        }

        private void Awake()
        {
            GameConfig.RegisterReader(this);
        }

        public IEnumerable<TextAsset> LoadConfigAssets() => Resources.LoadAll<TextAsset>(ConfigPath);
        
        public string GetConfigPath() => ConfigPath;
        public int GetPriority() => Priority;
    }

    public interface IConfigReader
    {
        IEnumerable<TextAsset> LoadConfigAssets();

        string GetConfigPath();
        int GetPriority();
    }
}