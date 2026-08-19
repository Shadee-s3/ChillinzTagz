using System;
using System.Collections.Generic;
using UnityEngine;

namespace Granalax.Mods
{
    public static class ModListDetection
    {
        public enum ModStatus
        {
            Legal,
            Illegal,
            Unknown
        }

        public class DetectedMod
        {
            public string Name;
            public ModStatus Status;

            public DetectedMod(
                string name,
                ModStatus status)
            {
                Name = name;
                Status = status;
            }
        }

        private static readonly HashSet<string> LegalMods =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "Utilla",
                "MonkeModManager",
                "ComputerInterface",
                "Bark",
                "Granalax HoloTags",
                "bingusnametagplusplus",
                "GorillaShirts",
                "GorillaShirtsBephInEx",
                "BodyTrack",
                "WalkSimulator",
                "WalkSim"
            };

        private static readonly HashSet<string> IllegalMods =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "SeralythMenu",
                "Atlas.Remade",
                "ShibaGTGoldReborn",
                "RexonMenuPaid",
                "RexonMenuFree",
                "xyfer",
                "Hamburbur",
                "Undefined",
                "juul",
                "CosmetX",
                "CosmetiX"
            };

        public static ModStatus CheckMod(
            string modName)
        {
            string name =
                CleanName(modName);

            if (string.IsNullOrEmpty(name))
                return ModStatus.Unknown;

            if (LegalMods.Contains(name))
                return ModStatus.Legal;

            if (IllegalMods.Contains(name))
                return ModStatus.Illegal;

            return ModStatus.Unknown;
        }

        public static ModStatus CheckMods(
            List<string> mods)
        {
            if (mods == null ||
                mods.Count == 0)
            {
                return ModStatus.Unknown;
            }

            bool foundLegal = false;

            foreach (string mod in mods)
            {
                ModStatus status =
                    CheckMod(mod);

                if (status == ModStatus.Illegal)
                    return ModStatus.Illegal;

                if (status == ModStatus.Legal)
                    foundLegal = true;
            }

            if (foundLegal)
                return ModStatus.Legal;

            return ModStatus.Unknown;
        }

        public static bool IsLegal(
            string modName)
        {
            return CheckMod(modName) ==
                   ModStatus.Legal;
        }

        public static bool IsIllegal(
            string modName)
        {
            return CheckMod(modName) ==
                   ModStatus.Illegal;
        }

        public static string GetStatusText(
            ModStatus status)
        {
            switch (status)
            {
                case ModStatus.Illegal:
                    return "CHEATER";

                case ModStatus.Unknown:
                    return "UNKNOWN";

                default:
                    return "LEGIT";
            }
        }

        public static Color GetStatusColor(
            ModStatus status)
        {
            switch (status)
            {
                case ModStatus.Illegal:
                    return new Color(
                        1f,
                        0.12f,
                        0.12f,
                        1f);

                case ModStatus.Unknown:
                    return new Color(
                        1f,
                        0.75f,
                        0.15f,
                        1f);

                default:
                    return new Color(
                        0.25f,
                        1f,
                        0.35f,
                        1f);
            }
        }

        private static string CleanName(
            string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";

            name =
                name.Trim();

            if (name.EndsWith(
                ".dll",
                StringComparison.OrdinalIgnoreCase))
            {
                name =
                    name.Substring(
                        0,
                        name.Length - 4);
            }

            return name;
        }
    }
}