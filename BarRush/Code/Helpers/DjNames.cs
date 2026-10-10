using Unity.Mathematics;

namespace BarRush
{
    // Invented DJ names. Any resemblance to a real act is a coincidence.
    public static class DjNames
    {
        private static readonly string[] s_First =
        {
            "Velvet", "Neon", "Midnight", "Static", "Chrome", "Lunar", "Sable", "Cobalt",
            "Echo", "Glitch", "Honey", "Voltage", "Paper", "Mirage", "Riot", "Tempo",
            "Marble", "Cinder", "Hollow", "Atlas",
        };

        private static readonly string[] s_Second =
        {
            "Fox", "Wave", "Pilot", "Bloom", "Circuit", "Drift", "Parade", "Ghost",
            "Sparrow", "Tide", "Engine", "Orchid", "Comet", "Harbor", "Signal", "Ember",
            "Lantern", "Rocket", "Mantis", "Velocity",
        };

        public static string Pick(ref Random random)
        {
            string a = s_First[random.NextInt(s_First.Length)];
            string b = s_Second[random.NextInt(s_Second.Length)];

            switch (random.NextInt(3))
            {
                case 0: return $"DJ {a} {b}";
                case 1: return $"{a} {b}";
                default: return $"DJ {b}-{a}";
            }
        }

        public struct MusicAct
        {
            public string Name;
            public string Genre;
            public string Billing;
        }

        private static readonly string[] s_BandNames =
        {
            "The Side Streets", "Paper Satellites", "Static Weekend", "Velvet Transit",
            "The Late Signals", "Harbor Lights", "Quiet Riot Club", "The Blue Hour",
            "Copper Avenue", "Midnight Assembly", "Northbound Echo", "Small Town Static",
        };

        private static readonly string[] s_SoloNames =
        {
            "Riley North", "June Wilder", "Avery Stone", "Milo Reed", "Casey Bloom",
            "Jordan Vale", "Remy Hart", "Sasha Lane", "Robin Ellis", "Taylor Moss",
        };

        public static MusicAct PickMusicAct(ref Random random)
        {
            switch (random.NextInt(7))
            {
                case 0: return new MusicAct { Name = Pick(ref random), Genre = "electronic", Billing = "DJ set" };
                case 1: return new MusicAct { Name = s_BandNames[random.NextInt(s_BandNames.Length)], Genre = "rock", Billing = "Rock band" };
                case 2: return new MusicAct { Name = s_BandNames[random.NextInt(s_BandNames.Length)], Genre = "indie", Billing = "Indie band" };
                case 3: return new MusicAct { Name = s_BandNames[random.NextInt(s_BandNames.Length)], Genre = "punk", Billing = "Punk band" };
                case 4: return new MusicAct { Name = s_BandNames[random.NextInt(s_BandNames.Length)], Genre = "folk", Billing = "Folk duo" };
                case 5: return new MusicAct { Name = s_BandNames[random.NextInt(s_BandNames.Length)], Genre = "jazz", Billing = "Jazz trio" };
                default: return new MusicAct { Name = s_SoloNames[random.NextInt(s_SoloNames.Length)], Genre = "acoustic", Billing = "Singer-songwriter" };
            }
        }
    }
}
