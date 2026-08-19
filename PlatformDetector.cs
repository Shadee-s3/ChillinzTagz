using System;
using System.Reflection;
using UnityEngine;

namespace Granalax.Mods
{
    public static class PlatformDetector
    {
        public static string GetPlatform(VRRig rig)
        {
            if (rig == null)
                return "UNKNOWN";

            try
            {
                object netPlayer =
                    rig.OwningNetPlayer;

                if (netPlayer == null)
                    return "UNKNOWN";

                Type netPlayerType =
                    netPlayer.GetType();

                PropertyInfo[] properties =
                    netPlayerType.GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic
                    );

                foreach (PropertyInfo property in properties)
                {
                    object value;

                    try
                    {
                        value =
                            property.GetValue(
                                netPlayer,
                                null
                            );
                    }
                    catch
                    {
                        continue;
                    }

                    if (value == null)
                        continue;

                    string result =
                        InspectObject(
                            property.Name,
                            value,
                            0
                        );

                    if (result != "UNKNOWN")
                        return result;
                }
            }
            catch
            {
            }

            return "UNKNOWN";
        }

        private static string InspectObject(
            string propertyName,
            object value,
            int depth)
        {
            if (value == null)
                return "UNKNOWN";

            if (depth > 2)
                return "UNKNOWN";

            string direct =
                CheckValue(
                    propertyName,
                    value
                );

            if (direct != "UNKNOWN")
                return direct;

            Type type =
                value.GetType();

            /*
             * Strings/enums can be checked directly.
             */
            if (value is string ||
                type.IsEnum)
            {
                return CheckValue(
                    propertyName,
                    value
                );
            }

            /*
             * Inspect properties of the nested
             * Photon/network object.
             */
            PropertyInfo[] properties =
                type.GetProperties(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                );

            foreach (PropertyInfo property in properties)
            {
                string name =
                    property.Name;

                object child;

                try
                {
                    child =
                        property.GetValue(
                            value,
                            null
                        );
                }
                catch
                {
                    continue;
                }

                if (child == null)
                    continue;

                string result =
                    CheckValue(
                        name,
                        child
                    );

                if (result != "UNKNOWN")
                    return result;

                /*
                 * Continue down another level.
                 */
                if (depth < 2)
                {
                    result =
                        InspectObject(
                            name,
                            child,
                            depth + 1
                        );

                    if (result != "UNKNOWN")
                        return result;
                }
            }

            return "UNKNOWN";
        }

        private static string CheckValue(
            string name,
            object value)
        {
            if (value == null)
                return "UNKNOWN";

            string key =
                name == null
                    ? ""
                    : name.ToLowerInvariant();

            string text =
                value.ToString()
                    .ToLowerInvariant();

            /*
             * QUEST / ANDROID
             */
            if (text.Contains("android") ||
                text.Contains("quest") ||
                text.Contains("oculusmobile"))
            {
                return "QUEST";
            }

            /*
             * STEAM / STEAMVR
             */
            if (text.Contains("steamvr") ||
                text.Contains("steam"))
            {
                return "STEAM";
            }

            /*
             * OCULUS PC
             */
            if (text.Contains("oculus") &&
                !text.Contains("mobile"))
            {
                return "OCULUS PC";
            }

            /*
             * Some network properties may have
             * generic names but contain PC/Windows.
             */
            if (text.Contains("windows") ||
                text.Contains("standalonewindows") ||
                text == "pc")
            {
                return "PC";
            }

            /*
             * Property-name checks.
             */
            if (key.Contains("platform"))
            {
                if (text.Contains("quest") ||
                    text.Contains("android"))
                {
                    return "QUEST";
                }

                if (text.Contains("steam"))
                {
                    return "STEAM";
                }

                if (text.Contains("oculus"))
                {
                    return "OCULUS PC";
                }

                if (text.Contains("pc") ||
                    text.Contains("windows"))
                {
                    return "PC";
                }
            }

            return "UNKNOWN";
        }

        public static string GetPlatformName(
            VRRig rig)
        {
            return GetPlatform(rig);
        }
    }
}