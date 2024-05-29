using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;
using log4net;

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
    [HarmonyPatch(typeof(Player), nameof(Player.EarnXP), new Type[] { typeof(long), typeof(XpType), typeof(ShareType) })]
    public static bool PreEarnXP(long amount, XpType xpType, ShareType shareType, ref Player __instance)
    {
        if (__instance.CurrentLandblock.CurrentLandblockGroup.Heat > 0 && xpType == XpType.Kill)
        {
            var xpMultiplier = GetXpMultiplier(__instance.CurrentLandblock.CurrentLandblockGroup);
            //Console.WriteLine($"{Name}.EarnXP({amount}, {sharable}, {fixedAmount})");

            // apply xp modifiers.  Quest XP is multiplicative with general XP modification
            var questModifier = PropertyManager.GetDouble("quest_xp_modifier").Item;
            var modifier = PropertyManager.GetDouble("xp_modifier").Item;
            if (xpType == XpType.Quest)
                modifier *= questModifier;

            // should this be passed upstream to fellowship / allegiance?
            var enchantment = __instance.GetXPAndLuminanceModifier(xpType);

            var m_amount = (long)Math.Round(amount * enchantment * modifier) + (long)(amount * xpMultiplier);

            if (m_amount < 0)
            {
                return true;
            }

            if (__instance.Hardcore == true)
            {
                m_amount = m_amount + (long)(m_amount * 0.50);
            }

            __instance.GrantXP(m_amount, xpType, shareType);
            return false;
        }
        else
            return true;
    }

    //this returns the XP multiplier based on the current landblock heat
    public static float GetXpMultiplier(LandblockGroup landblockGroup)
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



