using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.WorldObjects;

namespace ValHeelLandblockHeatMod;

internal class PlayerLuminancePatch
{
    #region Settings
    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);
    #endregion

    #region Patch

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), "EarnLuminance", new Type[] { typeof(long), typeof(XpType), typeof(ShareType) })]
    public static bool PreEarnLuminance(long amount, XpType xpType, ShareType shareType, ref Player __instance)
    {
        if (__instance.CurrentLandblock.CurrentLandblockGroup.Heat > 0)
        {
            var luminanceMultiplier = GetLuminanceMultiplier(__instance.CurrentLandblock.CurrentLandblockGroup);
            amount = amount + (long)(amount * luminanceMultiplier);
        }
        return false;
    }


    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(Player), "EarnLuminance", new Type[] { typeof(long) })]
    public static void PostEarnLumenance(long amount, ref Player __instance)
    {
        if (__instance.CurrentLandblock.CurrentLandblockGroup.Heat > 0)
        {
            var luminanceMultiplier = GetLuminanceMultiplier(__instance.CurrentLandblock.CurrentLandblockGroup);
            amount = amount + (long)(amount * luminanceMultiplier);
        }
    }*/

    public static float GetLuminanceMultiplier(LandblockGroup landblockGroup)
    {
        if (landblockGroup.Heat >= 0 && landblockGroup.Heat <= 2999)
            return Settings.XpMultiplier;
        else if (landblockGroup.Heat >= 3000 && landblockGroup.Heat <= 5999)
            return Settings.XpMultiplier * 2;
        else if (landblockGroup.Heat >= 6000 && landblockGroup.Heat <= 8999)
            return Settings.XpMultiplier * 3;
        else if (landblockGroup.Heat >= 9000 && landblockGroup.Heat <= 11999)
            return Settings.XpMultiplier * 4;
        else if (landblockGroup.Heat >= 12000 && landblockGroup.Heat <= 15000)
            return Settings.XpMultiplier * 5;
        else
            return 0;
    }

    #endregion
}



