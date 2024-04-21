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
            HandleHeat(landblockgroup);
            AdjustDecacyRate(landblockgroup);
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

        //If the last heat decay tick is more than 5 seconds ago, track the heat trend
        if (__instance.LastHeatDecayTick + 5 < currentUnixTime)
            TrackHeatTrend(__instance);
        
        //If the last heat decay tick is more than the heat decay rate, decrement the heat
        if (__instance.LastHeatDecayTick + Settings.HeatDecayRate < currentUnixTime)
        {
            //If the heat is greater than 0, set lastheat to heat then decrement the heat
            if (__instance.Heat > 0)
            {
                __instance.LastHeat = __instance.Heat;
                __instance.Heat--;
            }

            //Set the last heat decay tick to the current time
            __instance.LastHeatDecayTick = currentUnixTime;
        }
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
            var players = landblock.GetWorldObjectsForPhysicsHandling().OfType<Player>();
            var playerCount = players.Count();

            if (playerCount > 1)
            {
                //Set the heat decay rate to 6 + 0.5 per player
                Settings.HeatDecayRate = 6.0 + (playerCount * 0.5);

                //If the heat decay rate is less than 0.1, set it to 0.2
                if (Settings.HeatDecayRate < 0.1)
                    Settings.HeatDecayRate = 0.2;
            }
            //If there is only one player in the landblock, set the heat decay rate to 6 else set it to 1
            else if (playerCount == 1)
                Settings.HeatDecayRate = 6.0;
            else
                Settings.HeatDecayRate = 1;
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

