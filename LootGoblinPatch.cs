using ACE.Common;
using ACE.Database;
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
        //Debugger.Break();
        var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        //Roll for goblin chance
        var goblinRoll = ThreadSafeRandom.Next(0f, 1f);

        //If the roll is greater than the goblin chance, spawn the goblin
        if (goblinRoll < Settings.GoblinChance && currentUnixTime - p.LootGoblinTimeStamp > Settings.LootGoblinTimer) //3600 is 1 hour in seconds
        {
            if (p.Level >= Settings.T1GoblinPLayerLevel && p.Level < Settings.T2GoblinPLayerLevel)
            {
                var goblin = Settings.LootGoblin1Wcid;
                var goblinWeenie = DatabaseManager.World.GetCachedWeenie((uint)goblin);
                var newGoblin = WorldObjectFactory.CreateNewWorldObject(goblinWeenie);

                newGoblin.Location = p.Location;
                newGoblin.Location.LandblockId = new LandblockId(newGoblin.Location.GetCell());
                newGoblin.EnterWorld();
                p.SendMessage("A Loot Drudge has appeared!");
                p.LootGoblinTimeStamp = currentUnixTime;
                newGoblin.PlayParticleEffect(PlayScript.AetheriaLevelUp, newGoblin.Guid);
                newGoblin.PlaySoundEffect(Sound.UI_Bell, newGoblin.Guid);
            }
            else if (p.Level >= Settings.T2GoblinPLayerLevel && p.Level < Settings.T3GoblinPLayerLevel)
            {
                var goblin = Settings.LootGoblin2Wcid;
                var goblinWeenie = DatabaseManager.World.GetCachedWeenie((uint)goblin);
                var newGoblin = WorldObjectFactory.CreateNewWorldObject(goblinWeenie);

                newGoblin.Location = p.Location;
                newGoblin.Location.LandblockId = new LandblockId(newGoblin.Location.GetCell());
                newGoblin.EnterWorld();
                p.SendMessage("A Loot Drudge has appeared!");
                p.LootGoblinTimeStamp = currentUnixTime;
                newGoblin.PlayParticleEffect(PlayScript.AetheriaLevelUp, newGoblin.Guid);
                newGoblin.PlaySoundEffect(Sound.UI_Bell, newGoblin.Guid);
            }
            else if (p.Level >= Settings.T3GoblinPLayerLevel && p.Level < Settings.T4GoblinPLayerLevel)
            {
                var goblin = Settings.LootGoblin3Wcid;
                var goblinWeenie = DatabaseManager.World.GetCachedWeenie((uint)goblin);
                var newGoblin = WorldObjectFactory.CreateNewWorldObject(goblinWeenie);

                newGoblin.Location = p.Location;
                newGoblin.Location.LandblockId = new LandblockId(newGoblin.Location.GetCell());
                newGoblin.EnterWorld();
                p.SendMessage("A Loot Drudge has appeared!");
                p.LootGoblinTimeStamp = currentUnixTime;
                newGoblin.PlayParticleEffect(PlayScript.AetheriaLevelUp, newGoblin.Guid);
                newGoblin.PlaySoundEffect(Sound.UI_Bell, newGoblin.Guid);
            }
            else if (p.Level >= Settings.T4GoblinPLayerLevel)
            {
                var maxGoblinWeenie = DatabaseManager.World.GetCachedWeenie((uint)Settings.LootGoblin4Wcid);
                var newMaxGoblin = WorldObjectFactory.CreateNewWorldObject(maxGoblinWeenie);

                newMaxGoblin.Location = p.Location;
                newMaxGoblin.Location.LandblockId = new LandblockId(newMaxGoblin.Location.GetCell());
                newMaxGoblin.EnterWorld();
                p.SendMessage("A Loot Drudge has appeared!");
                p.LootGoblinTimeStamp = currentUnixTime;
                newMaxGoblin.PlayParticleEffect(PlayScript.AetheriaLevelUp, newMaxGoblin.Guid);
                newMaxGoblin.PlaySoundEffect(Sound.UI_Bell, newMaxGoblin.Guid);
            }
        }
    }

    #endregion
}

