using System.Collections.Generic;
using ACE.Server.Entity;
using ACE.Server.WorldObjects;

namespace ValHeelLandblockHeatMod;

[HarmonyPatchCategory(nameof(CreatureDeathPatch))]
internal class CreatureDeathPatch
{
    #region Settings
    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);
    #endregion

    #region Patch

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), "Die", new Type[] { typeof(DamageHistoryInfo), typeof(DamageHistoryInfo) })]

    /// <summary>
    /// This is the post-die patch for creatures
    /// </summary>
    /// <see cref="Creature.Die(DamageHistoryInfo, DamageHistoryInfo)"/>"/>
    /// <see cref="DamageHistoryInfo"/>"/>
    /// <see cref="Creature"/>"/>
    /// <see cref="LandblockGroup"/>"/>

    public static void PostDie(DamageHistoryInfo lastDamager, DamageHistoryInfo topDamager, ref Creature __instance)
    {
        var landblockGroup = __instance.CurrentLandblock.CurrentLandblockGroup;

        if (!__instance.IsCombatPet && landblockGroup.Heat < Settings.LandblockHeatCap)
            landblockGroup.Heat++;
        
        var killer = lastDamager.TryGetAttacker();

        if (killer is Player killerPlayer && !killerPlayer.IsOlthoiPlayer())
        {
            if (__instance.CurrentLandblock.CurrentLandblockGroup.Heat > Settings.GolbinHeatThreshold)
                LootGoblinPatch.RollForLootGoblin(__instance.CurrentLandblock.CurrentLandblockGroup, killerPlayer);
        }
    }

    #endregion
}

