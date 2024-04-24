using ACE.Common;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.WorldObjects;
using log4net;
using log4net.Appender;

namespace ValHeelLandblockHeatMod;

[HarmonyPatchCategory(nameof(LandblockPatch))]
internal class LandblockPatch
{
    #region Settings
    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);
    #endregion

    #region Patch

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LandblockManager), "TickMultiThreadedWork")]

    /// <summary>
    /// This is the post-tick patch for landblocks
    /// </summary>
    public static void PostTickMultiThreadedWork()
    {
        foreach (var landblockgroup in LandblockManager.landblockGroups)
        {
            AdjustDecacyRate(landblockgroup);
            HandleHeat(landblockgroup);
        }
    }

    /// <summary>
    /// This is the Landblock Group heat handler
    /// </summary>
    /// <param name="__instance"></param>
    public static void HandleHeat(LandblockGroup __instance)
    {
        var currentUnixTime = Time.GetUnixTime();

        //If the last heat decay tick is 0, set it to the current time
        if (__instance.LastHeatDecayTick == 0)
            __instance.LastHeatDecayTick = currentUnixTime;

        if (__instance.LastHeatTrendTick == 0)
            __instance.LastHeatTrendTick = currentUnixTime;

        //If the last heat decay tick is more than the heat decay rate, decrement the heat
        if (__instance.LastHeatDecayTick + __instance.BaseHeatDecayRate < currentUnixTime)
        {
            //If the heat is greater than 0, set lastheat to heat then decrement the heat
            if (__instance.Heat > 0)
            {
                __instance.Heat--;
            }

            //Set the last heat decay tick to the current time
            __instance.LastHeatDecayTick = currentUnixTime;

            if (__instance.Heat > __instance.LastHeat)
            {
                __instance.LastHeat = __instance.Heat;
            }

            //If last heat is greater than heat by 10, decrement last heat.
            if (__instance.LastHeat > __instance.Heat + 3)
            {
                __instance.LastHeat--;
            }

            if (__instance.Heat == 0)
                __instance.LastHeat = 0;

            __instance.LastHeatTrendTick = currentUnixTime;
        }

        TrackHeatTrend(__instance);
    }

    /// <summary>
    /// This is the heat decay rate adjuster
    /// </summary>
    /// <param name="__instance"></param>
    public static void AdjustDecacyRate(LandblockGroup __instance)
    {
        foreach (var landblock in __instance)
        {
            //Get all players in the landblock and count them
            var players = landblock.GetAllWorldObjectsForDiagnostics().OfType<Player>();
            var playerCount = players.Count();

            if (playerCount > 1)
            {
                //Set the heat decay rate to Settings.BaseHeatDecayRate + 0.5 per player
                __instance.BaseHeatDecayRate = 7.0 + (playerCount * 0.5);

                //If the heat decay rate is less than 1, set it to 1
                if (__instance.BaseHeatDecayRate < 1)
                    __instance.BaseHeatDecayRate = 1;
            }
            //If there is only one player in the landblock, set the heat decay rate to 7 else set it to 1
            if (playerCount == 1)
                __instance.BaseHeatDecayRate = 7.0;
        }
    }

    /// <summary>
    /// This is the heat trend tracker
    /// </summary>
    /// <param name="__instance"></param>
    public static void TrackHeatTrend(LandblockGroup __instance)
    {
        //If the heat is greater than the last heat, set the trend to increasing
        //If the heat is less than the last heat, set the trend to decreasing
        //If the heat is the same as the last heat, set the trend to stable
        if (__instance.Heat > __instance.LastHeat)
            __instance.CurrentHeatTrend = LandblockGroup.HeatTrend.Increasing;
        else if (__instance.Heat < __instance.LastHeat)
            __instance.CurrentHeatTrend = LandblockGroup.HeatTrend.Decreasing;
        else
            __instance.CurrentHeatTrend = LandblockGroup.HeatTrend.Stable;
    }
    
    #endregion
}

