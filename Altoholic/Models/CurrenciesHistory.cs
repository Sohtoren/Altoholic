namespace Altoholic.Models
{
    public class CurrenciesHistory
    {
        public ulong CharacterId { get; init; } = 0;
        public PlayerCurrencies? Currencies { get; set; }
        public long Datetime { get; init; } = 0;
    }
}
