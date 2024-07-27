namespace Altoholic.Cache
{
    public class GlobalCache
    {
        public required IconStorage IconStorage { get; set; } = null!;
        public required ItemStorage ItemStorage { get; set; } = null!;
        public required JobStorage JobStorage{ get; set; } = null!;
        public required AddonStorage AddonStorage { get; set; } = null!;
        public required StainStorage StainStorage { get; set; } = null!;
        public required MinionStorage MinionStorage { get; set; } = null!;
        public required MountStorage MountStorage { get; set; } = null!;
        public required TripleTriadCardStorage TripleTriadCardStorage { get; set; } = null!;
        public required EmoteStorage EmoteStorage { get; set; } = null!;
        public required BardingStorage BardingStorage { get; set; } = null!;
        public required FramerKitStorage FramerKitStorage { get; set; } = null!;
        public required OrchestrionRollStorage OrchestrionRollStorage { get; set; } = null!;
        public required OrnamentStorage OrnamentStorage { get; set; } = null!;
        public required GlassesStorage GlassesStorage { get; set; } = null!;
    }
}
