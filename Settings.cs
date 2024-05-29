using ACE.Entity.Enum.Properties;

namespace ValHeelLandblockHeatMod
{
    public class Settings
    {
        public int LootGoblin1Wcid { get; set; } = 803562;
        public int LootGoblin2Wcid { get; set; } = 803563;
        public int LootGoblin3Wcid { get; set; } = 803564;
        public int LootGoblin4Wcid { get; set; } = 803565;
        public float GoblinChance { get; set; } = 0.001f;
        public int T1GoblinPLayerLevel { get; set; } = 275;
        public int T2GoblinPLayerLevel { get; set; } = 400;
        public int T3GoblinPLayerLevel { get; set; } = 1499;
        public int T4GoblinPLayerLevel { get; set; } = 1500;
        public int GolbinHeatThreshold { get; set; } = 14500;
        public int LandblockHeatCap { get; set; } = 15000;
        public float LootGoblinTimer { get; set; } = 3600;
    }
}


