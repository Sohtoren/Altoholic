namespace Altoholic.Cache
{
    public class GlobalCache
    {
        public IconStorage IconStorage { get; set; } = null!;
        public ItemStorage ItemStorage { get; set; } = null!;
        public JobStorage JobStorage{ get; set; } = null!;
        public AddonStorage AddonStorage { get; set; } = null!;
        public StainStorage StainStorage { get; set; } = null!;
    }
}
