using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACE.Common;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Entity;
using ACE.Server.WorldObjects;
using ValHeelLandblockHeatMod;

namespace ValHeelLandblockHeatMod;

[HarmonyPatchCategory(nameof(PlayerXpPatch))]

internal class PlayerXpPatch
{
    #region Settings
    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);
    #endregion

    #region Patch

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), "EarnXP", new Type[] { typeof(long), typeof(XpType), typeof(ShareType) })]
    public static bool PreEarnXP(long amount, XpType xpType, ShareType shareType, ref Player __instance)
    {
        double currentUnixTime = Time.GetUnixTime();

        if (__instance.CurrentLandblock.CurrentLandblockGroup.Heat > 0)
        {
            var xpMultiplier = GetXpMultiplier(__instance.CurrentLandblock.CurrentLandblockGroup);
            amount = amount + (long)(amount * xpMultiplier);
        }
        return false;
    }

    //this returns the XP multiplier based on the current landblock heat
    public static float GetXpMultiplier(LandblockGroup landblockGroup)
    {
        if (landblockGroup.Heat >= 0 && landblockGroup.Heat <= 2999)
            return 0.03f;
        else if (landblockGroup.Heat >= 3000 && landblockGroup.Heat <= 5999)
            return 0.06f;
        else if (landblockGroup.Heat >= 6000 && landblockGroup.Heat <= 8999)
            return 0.09f;
        else if (landblockGroup.Heat >= 9000 && landblockGroup.Heat <= 11999)
            return 0.12f;
        else if (landblockGroup.Heat >= 12000 && landblockGroup.Heat <= 15000)
            return 0.15f;
        else
            return 0;
    }

    #endregion
}



