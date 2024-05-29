using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;

namespace ValHeelLandblockHeatMod;

[HarmonyPatchCategory(nameof(PlayerLuminancePatch))]

internal class PlayerLuminancePatch
{
    #region Settings
    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);
    #endregion

    #region Patch

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Player), nameof(Player.EarnLuminance), new Type[] { typeof(long), typeof(XpType), typeof(ShareType) })]
    public static bool PreEarnLuminance(long amount, XpType xpType, ShareType shareType, ref Player __instance)
    {
        if (__instance.CurrentLandblock.CurrentLandblockGroup.Heat > 0)
        {
            var luminanceMultiplier = GetLuminanceMultiplier(__instance.CurrentLandblock.CurrentLandblockGroup);
            // following the same model as Player_Xp

            var modifier = PropertyManager.GetDouble("luminance_modifier").Item;

            // should this be passed upstream to fellowship?
            var enchantment = __instance.GetXPAndLuminanceModifier(xpType);

            var m_amount = (long)Math.Round(amount * enchantment * modifier) + (long)(amount * luminanceMultiplier);

            __instance.GrantLuminance(m_amount, xpType, shareType);
            return false;
        }
        else
            return true;
    }


    public static float GetLuminanceMultiplier(LandblockGroup landblockGroup)
    {
        if (landblockGroup.Heat >= 1000 && landblockGroup.Heat <= 2999)
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



