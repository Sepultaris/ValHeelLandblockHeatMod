namespace ValHeelLandblockHeatMod
{
    public class Settings
    {
        public double BaseHeatDecayRate { get; set; } = 7.0d;
        public float LootGoblinChance { get; set; } = 5.0f;
        public int LootGoblin1Wcid { get; set; } = 803562;
        public int LootGoblin2Wcid { get; set; } = 803563;
        public int LootGoblin3Wcid { get; set; } = 803564;
        public int LootGoblin4Wcid { get; set; } = 803565;
        public int GoblinChance { get; set; } = 86000;
        public int T1GoblinPLayerLevel { get; set; } = 275;
        public int T2GoblinPLayerLevel { get; set; } = 400;
        public int T3GoblinPLayerLevel { get; set; } = 1499;
        public int T4GoblinPLayerLevel { get; set; } = 1500;
        public int GolbinHeatThreshold { get; set; } = 14500;
        public int LandblockHeatCap { get; set; } = 15000;
    }
}


