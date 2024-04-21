using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACE.Database;
using ACE.Database.Models.World;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Factories;
using ACE.Server.WorldObjects;

namespace ValHeelLandblockHeatMod;

[HarmonyPatchCategory(nameof(LootGoblinPatch))]
internal class LootGoblinPatch
{
    #region Settings
    public static Settings Settings = new();
    static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
    private FileInfo settingsInfo = new(settingsPath);
    #endregion

    #region Patch

    /// <summary>
    /// This method will roll for a loot goblin to spawn in the landblock based on the player level
    /// </summary>
    /// <param name="__instance"></param>
    public static void RollForLootGoblin(LandblockGroup __instance, Player p)
    {
        //Goblin dictionary with key being the level of the player and value belonging to the goblin wcid
        Dictionary<int, int> goblins = new()
        {
            {Settings.T1GoblinPLayerLevel, Settings.LootGoblin1Wcid },
            {Settings.T2GoblinPLayerLevel, Settings.LootGoblin2Wcid },
            {Settings.T2GoblinPLayerLevel, Settings.LootGoblin3Wcid },
            {Settings.T4GoblinPLayerLevel, Settings.LootGoblin4Wcid }
        };

        //Roll for goblin chance
        var goblinRoll = new Random().Next(1, Settings.GoblinChance);

        //If the roll is greater than the goblin chance, spawn the goblin
        if (goblinRoll >= Settings.GoblinChance)
        {
            //If the player level is less than goblins key, spawn the goblin
            var goblin = goblins.Where(x => x.Key <= p.Level).OrderByDescending(x => x.Key).FirstOrDefault().Value;
            var goblinWeenie = DatabaseManager.World.GetCachedWeenie((uint)goblin);
            var newGoblin = WorldObjectFactory.CreateNewWorldObject(goblinWeenie);

            newGoblin.Location = p.Location;
            newGoblin.Location.LandblockId = new LandblockId(newGoblin.Location.GetCell());
            newGoblin.EnterWorld();
            p.SendMessage("A Loot Drudge has appeared!");

            //If the player level is 1500 or greater, spawn the t4 goblin
            if (p.Level > Settings.T4GoblinPLayerLevel)
            {
                var maxGoblinWeenie = DatabaseManager.World.GetCachedWeenie((uint)Settings.LootGoblin4Wcid);
                var newMaxGoblin = WorldObjectFactory.CreateNewWorldObject(maxGoblinWeenie);

                newMaxGoblin.Location = p.Location;
                newMaxGoblin.Location.LandblockId = new LandblockId(newGoblin.Location.GetCell());
                newMaxGoblin.EnterWorld();
                p.SendMessage("A Loot Drudge has appeared!");
            }
        }
    }

    #endregion
}

