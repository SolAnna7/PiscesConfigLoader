using System.Linq;
using UnityEditor;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Utils
{
    [InitializeOnLoad]
    public class DefinePiscesSymbols
    {
        private static readonly string CONFIG_DEFINE_SYMBOL = "SFG_PISCES_CONFIG";

        static DefinePiscesSymbols()
        {
            EditorApplication.update += OnEditorApplicationUpdate;
        }

        private static bool IsSymbolDefined(string symbol, BuildTargetGroup target) =>
            PlayerSettings.GetScriptingDefineSymbolsForGroup(target)
                .Split(';')
                .Any(s => s == symbol);

        private static void OnEditorApplicationUpdate()
        {
#if !SFG_PISCES_CONFIG
            var target = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (!IsSymbolDefined(CONFIG_DEFINE_SYMBOL, target))
            {
                string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, currentSymbols + ";" + CONFIG_DEFINE_SYMBOL);
            }
#endif
        }
    }
}
