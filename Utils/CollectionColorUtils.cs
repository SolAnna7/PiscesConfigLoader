using System;
using UnityEngine;

namespace SnowFlakeGamesAssets.PiscesConfigLoader.Utils
{
    public class CollectionColorUtils
    {
        public static Color ParseColorFromHtml(string htmlColor)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(htmlColor, out color))
                return color;
            throw new Exception($"Unparsable color string: {htmlColor}");
        }
    }
}