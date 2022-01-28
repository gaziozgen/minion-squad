using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public static class ColorExtensions
    {
        public static float GetHue(this Color color)
        {
            Color.RGBToHSV(color, out float H, out _, out _);
            return H;
        }

        public static void SetHue(this ref Color color, float newHue)
        {
            Color.RGBToHSV(color, out _, out float S, out float V);
            color = Color.HSVToRGB(newHue / 360f, S, V);
        }

        public static float GetSaturation(this Color color)
        {
            Color.RGBToHSV(color, out _, out float S, out _);
            return S;
        }

        public static void SetSaturation(this ref Color color, float newSaturation)
        {
            Color.RGBToHSV(color, out float H, out _, out float V);
            color = Color.HSVToRGB(H, newSaturation, V);
        }
        public static float GetValue(this Color color)
        {
            Color.RGBToHSV(color, out _, out _, out float V);
            return V;
        }

        public static void SetValue(this ref Color color, float newValue)
        {
            Color.RGBToHSV(color, out float H, out float S, out _);
            color = Color.HSVToRGB(H, S, newValue);
        }

        public static void SetAlpha(this ref Color color, float newAlpha)
        {
            color = new Color(color.r, color.g, color.b, newAlpha);
        }
    }

}
