using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Command;
using ACE.Server.Network;
using ACE.Server.WorldObjects;

namespace ValHeelLandblockHeatMod
{
    [HarmonyPatch]
    public class PatchClass
    {
        #region Settings
        const int RETRIES = 10;

        public static Settings Settings = new();
        static string settingsPath => Path.Combine(Mod.ModPath, "Settings.json");
        private FileInfo settingsInfo = new(settingsPath);

        private JsonSerializerOptions _serializeOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        private void SaveSettings()
        {
            string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

            if (!settingsInfo.RetryWrite(jsonString, RETRIES))
            {
                ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
            }
        }

        private void LoadSettings()
        {
            if (!settingsInfo.Exists)
            {
                ModManager.Log($"Creating {settingsInfo}...");
                SaveSettings();
            }
            else
                ModManager.Log($"Loading settings from {settingsPath}...");

            if (!settingsInfo.RetryRead(out string jsonString, RETRIES))
            {
                Mod.State = ModState.Error;
                return;
            }

            try
            {
                Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
            }
            catch (Exception)
            {
                ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
                return;
            }
        }
        #endregion

        #region Start/Shutdown
        public void Start()
        {
            //Need to decide on async use
            Mod.State = ModState.Loading;
            LoadSettings();

            if (Mod.State == ModState.Error)
            {
                ModManager.DisableModByPath(Mod.ModPath);
                return;
            }

            Mod.State = ModState.Running;
        }

        public void Shutdown()
        {
            //if (Mod.State == ModState.Running)
            // Shut down enabled mod...

            //If the mod is making changes that need to be saved use this and only manually edit settings when the patch is not active.
            SaveSettings();

            if (Mod.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion

        #region Patches

        PropertyFloat lootGoblinTimestamp = (PropertyFloat)61000;

        /// <summary>
        /// This command will display the current heat of the landblock and the last heat decay tick.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="parameters"></param>
        [CommandHandler("heat", AccessLevel.Player, CommandHandlerFlag.RequiresWorld)]
        
        public static void HeatHandler(Session session, string[] parameters)
        {
            var player = session.Player;
            var heat = player.CurrentLandblock.CurrentLandblockGroup.Heat;
            var lastHeatDecayTick = player.CurrentLandblock.CurrentLandblockGroup.LastHeatDecayTick;
            var lastHeat = player.CurrentLandblock.CurrentLandblockGroup.LastHeat;
            var lastHeatTrendTick = player.CurrentLandblock.CurrentLandblockGroup.LastHeatTrendTick;
            var currentHeatDecayRate = player.CurrentLandblock.CurrentLandblockGroup.BaseHeatDecayRate;
            var players = player.CurrentLandblock.GetWorldObjectsForPhysicsHandling().OfType<Player>();
            var playerCount = players.Count();
            var drudgeHeatLevel = Settings.GolbinHeatThreshold;

            if (parameters.Length == 0)
            {
                player.SendMessage($"The Battle Heat in this area is {heat}. Heat is {player.CurrentLandblock.CurrentLandblockGroup.CurrentHeatTrend}");

                return;
            }
            if (parameters[0] == "help")
            {
                player.SendMessage("Usage:/heat Displays the current Battle Heat and trend.");
                player.SendMessage("Battle Heat increases with the number of monsters killed in an area.");
                player.SendMessage("If Battle Heat is high enough, any players in the area will gain an xp bonus when they kill monsters.");
                player.SendMessage("The xp bonuses are devided into tiers.");
                player.SendMessage("<Tier> <kills> <bonus>");
                player.SendMessage("   1    1000     3%   ");
                player.SendMessage("   2    3000     6%   ");
                player.SendMessage("   3    6000     9%   ");
                player.SendMessage("   4    9000    12%   ");
                player.SendMessage("   5   12000    15%   ");
                player.SendMessage($"When Battle Heat is above {drudgeHeatLevel} every kill has a chance to spawn a Loot Drudge next to the player.");
            }
        }
        #endregion
    }
}