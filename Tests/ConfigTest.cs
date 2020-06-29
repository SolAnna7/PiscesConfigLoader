// using System.Collections;
// using System.Linq;
// using NUnit.Framework;
// using SnowFlakeGamesAssets.PiscesConfigLoader.Component;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// namespace SnowFlakeGamesAssets.PiscesConfigLoader.Tests
// {
//     public class ConfigTest 
//     {
//         private readonly GameObject rootObj = new GameObject("root",
//             typeof(ConfigReaderComponent)
//         );
//
//
//         [UnityTest]
//         public IEnumerator TestConfigAssetLoader()
//         {
//             var root = Object.Instantiate(rootObj);
//             Assert.NotNull(root);
//             var assets = rootObj.GetComponent<ConfigReaderComponent>().LoadConfigAssets();
//
//             Assert.NotNull(assets);
//             Assert.True(assets.Any());
//
//             return null;
//         }
//
//     }
// }