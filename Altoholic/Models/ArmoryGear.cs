using System.Collections.Generic;

namespace Altoholic.Models
{
    public class ArmoryGear
    {
        public required List<Gear> MainHand { get; set; }
        public required List<Gear> Head { get; set; }
        public required List<Gear> Body { get; set; }
        public required List<Gear> Hands { get; set; }
        public required List<Gear> Legs { get; set; }
        public required List<Gear> Feets { get; set; }
        public required List<Gear> OffHand { get; set; }
        public required List<Gear> Ear { get; set; }
        public required List<Gear> Neck { get; set; }
        public required List<Gear> Wrist { get; set; }
        public required List<Gear> Rings { get; set; }
        public required List<Gear> SoulCrystal { get; set; }
    }
}
