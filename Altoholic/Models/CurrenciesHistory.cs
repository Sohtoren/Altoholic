using System.Collections.Generic;

namespace Altoholic.Models
{
    public class CurrenciesHistory
    {
        public ulong CharacterId { get; init; } = 0;
        public Dictionary<uint, int> Currencies { get; init; } = [];
        public long Datetime { get; init; } = 0;
    }
}
