namespace Altoholic.Models
{
    public class BaseParam
    {
        public uint Id { get; set; }
        public string GermanName { get; set; } = string.Empty;
        public string GermanDescription { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string EnglishShortName { get; set; } = string.Empty;
        public string EnglishDescription { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string FrenchShortName { get; set; } = string.Empty;
        public string FrenchDescription { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public string JapaneseShortName { get; set; } = string.Empty;
        public string JapaneseDescription { get; set; } = string.Empty;
        public ushort OneHandWeaponPercent { get; set; }
        public ushort OffHandPercent { get; set; }
        public ushort HeadPercent { get; set; }
        public ushort ChestPercent { get; set; }
        public ushort HandsPercent { get; set; }
        public ushort WaistPercent { get; set; }
        public ushort LegsPercent { get; set; }
        public ushort FeetPercent { get; set; }
        public ushort EarringPercent { get; set; }
        public ushort NecklacePercent { get; set; }
        public ushort BraceletPercent { get; set; }
        public ushort RingPercent { get; set; }
        public ushort TwoHandWeaponPercent { get; set; }
        public ushort UnderArmorPercent { get; set; }
        public ushort ChestHeadPercent { get; set; }
        public ushort ChestHeadLegsFeetPercent { get; set; }
        public ushort Unknown0 { get; set; }
        public ushort LegsFeetPercent { get; set; }
        public ushort HeadChestHandsLegsFeetPercent { get; set; }
        public ushort ChestLegsGlovesPercent { get; set; }
        public ushort ChestLegsFeetPercent { get; set; }
        public ushort Unknown1 { get; set; }
        public byte OrderPriority { get; set; }
        public byte[] MeldParam { get; set; } = [];
        public sbyte PacketIndex { get; set; }
        public bool Unknown2 { get; set; }

    }
}
