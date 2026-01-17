using System.Collections.Generic;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureGearsetModule;

namespace Altoholic.Models
{
    public class GearSet
    {
        public byte Id;    // This may actually be set number, which is not _quite_ ID.
        public required string Name;
        public byte ClassJob;
        public byte GlamourSetLink;
        public short ItemLevel;
        /// <remarks>This is the BannerIndex, but offset by 1. If it's 0, the gearset is not linked to a banner.</remarks>
        public byte BannerIndex;
        public GearsetFlag Flags;
        public List<Gear> Gears = [];
        public ushort[] GlassesIds = [];
    }
}