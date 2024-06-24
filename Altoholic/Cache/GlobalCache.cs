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
    }
}
