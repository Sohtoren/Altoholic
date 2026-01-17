using System.Collections.Generic;
using System.Text.Json.Serialization;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureGearsetModule;

namespace Altoholic.Models
{
    public class GearSet
    {
        public byte Id { get; init; }    // This may actually be set number, which is not _quite_ ID.
        public string Name { get; init; } = string.Empty;
        public byte ClassJob { get; init; }
        public byte GlamourSetLink { get; init; }
        public short ItemLevel { get; init; }
        /// <remarks>This is the BannerIndex, but offset by 1. If it's 0, the gearset is not linked to a banner.</remarks>
        public byte BannerIndex { get; init; }
        [JsonIgnore]
        public GearsetFlag Flags { get; init; }
        public List<Gear> Gears { get; init; } = [];
        public ushort[] GlassesIds { get; init; } = [];
    }
}