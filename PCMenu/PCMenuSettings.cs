using UnityEngine;

namespace ChillzMenu
{
    public static class PCMenuSettings
    {
        public static Color CurrentColor =
            new Color(
                0.05f,
                1f,
                0.15f,
                0.45f
            );

        public static void ChangeColor(
            Color newColor)
        {
            CurrentColor =
                newColor;
        }
    }
}