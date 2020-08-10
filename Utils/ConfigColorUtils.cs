using System;
using UnityEngine;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Utils
{
    public static class ConfigColorUtils
    {
        public static Color ParseColorFromHtml(string htmlColor)
        {
            if (ColorUtility.TryParseHtmlString(htmlColor, out var color))
                return color;
            throw new Exception($"Unparsable color string: {htmlColor}");
        }
    }
}