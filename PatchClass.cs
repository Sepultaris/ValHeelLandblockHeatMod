using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Command;
using ACE.Server.Entity;
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

            if (parameters.Length == 0)
            {
                player.SendMessage($"The current heat is: {heat} Heat is {player.CurrentLandblock.CurrentLandblockGroup.CurrentHeatTrend}");
                player.SendMessage($"The last heat decay tick was {lastHeatDecayTick}");
                player.SendMessage($"The last heat was {lastHeat}");
                player.SendMessage($"The last heat trend tick was {lastHeatTrendTick}");
                player.SendMessage($"The current heat decay rate is {currentHeatDecayRate}");
                player.SendMessage($"There are {playerCount} in land landblock");

                return;
            }
        }

        /// <summary>
        /// This command will set the heat of the landblock.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="parameters"></param>
        [CommandHandler("setheat", AccessLevel.Admin, CommandHandlerFlag.RequiresWorld)]

        public static void SetHeatHandler(Session session, string[] parameters)
        {
            var player = session.Player;

            if (parameters.Length == 0)
            {
                player.SendMessage("Usage: /setheat <value>");
                return;
            }

            if (int.TryParse(parameters[0], out int heat))
            {
                if (heat < 0 || heat > 15000)
                {
                      player.SendMessage("Invalid value");
                    return;
                }

                player.CurrentLandblock.CurrentLandblockGroup.Heat = heat;
                player.SendMessage($"Heat set to {heat}");
            }
            else
                player.SendMessage("Invalid value");
        }
        #endregion
    }
}