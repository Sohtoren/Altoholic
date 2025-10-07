using System.Collections.Generic;

namespace Altoholic.Models
{
    public class ArmoryGear
    {
        public required List<Gear> MainHand { get; init; }
        public required List<Gear> Head { get; init; }
        public required List<Gear> Body { get; init; }
        public required List<Gear> Hands { get; init; }
        public required List<Gear> Legs { get; init; }
        public required List<Gear> Feets { get; init; }
        public required List<Gear> OffHand { get; init; }
        public required List<Gear> Ear { get; init; }
        public required List<Gear> Neck { get; init; }
        public required List<Gear> Wrist { get; init; }
        public required List<Gear> Rings { get; init; }
        public required List<Gear> SoulCrystal { get; init; }
    }
}
