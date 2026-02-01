using FFXIVClientStructs.FFXIV.Client.Game;

namespace Altoholic.Models
{
    public class GlamourItem
    {
        public uint ItemId { get; init; } = 0;
        public short Slot { get; init; } = 0;
        public uint GlamourId { get; set; } = 0;
        public ushort Stain0 { get; init; } = 0;
        public ushort Stain1 { get; init; } = 0;
        public InventoryItem.ItemFlags Flags { get; init; }

        public bool IsSame(GlamourItem? otherItem)
        {
            if (otherItem is null)
            {
                return false;
            }
            if (Slot != otherItem.Slot)
            {
                return false;
            }

            if (ItemId != otherItem.ItemId)
            {
                return false;
            }

            if (Flags != otherItem.Flags)
            {
                return false;
            }

            if (Stain0 != otherItem.Stain0)
            {
                return false;
            }

            if (Stain1 != otherItem.Stain1)
            {
                return false;
            }

            if (GlamourId != otherItem.GlamourId)
            {
                return false;
            }

            return true;
        }
    }
}