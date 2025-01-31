using System.Collections.Generic;
using BepInEx.Bootstrap;
using RiskOfOptions;
using UnityEngine;
using ZioConfigFile;
using ZioRiskOfOptions;

namespace Aerolt.Helpers
{
    public static class Colors
    {
        public static readonly Dictionary<string, Color32> DefaultColors = new()
        {
            {"Chest", Color.red},
            {"Shop", Color.red},
            {"Secret_Plates", Color.cyan},
            {"Barrels", new Color32(255, 128, 0, 255)},
            {"Scrappers", Color.blue},
            {"NewtAlter", Color.white},
            {"Shrine", Color.white},
            {"Drone", Color.white},
            {"Printer", Color.white}
        };

        public static readonly Dictionary<string, ZioConfigEntry<Color>> GlobalColors = new();

        public static string GenerateColoredString(string text, Color color)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + text + "</color>";
        }

        public static Color32 GetColor(string identifier)
        {
            if (Load.ConfigFile == null) return Color.magenta;
            if (GlobalColors.TryGetValue(identifier, out var color)) return color.Value;

            color = Load.ConfigFile.Bind("EspColors", identifier, (Color) DefaultColors[identifier], "");
            GlobalColors[identifier] = color;
            if (Chainloader.PluginInfos.ContainsKey("bubbet.zioriskofoptions"))
                MakeRiskOfOptions(color);
            return color.Value;
        }

        private static void MakeRiskOfOptions(ZioConfigEntry<Color> value)
        {
            ModSettingsManager.AddOption(new ZioColorOption(value));
        }

        public static void SetColor(string id, Color32 c)
        {
            GlobalColors[id].Value = c;
        }

        public static string ColorToHex(Color32 color)
        {
            var hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        public static void InitColors()
        {
            foreach (var key in DefaultColors.Keys) GetColor(key);
        }
    }
}