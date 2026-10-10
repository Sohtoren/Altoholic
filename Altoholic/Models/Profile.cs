namespace Altoholic.Models
{
    public class Profile
    {
        public string Title { get; init; } = string.Empty;
        public bool TitleIsPrefix { get; init; } = false;
        public int GrandCompany { get; init; } = 0;
        public int GrandCompanyRank { get; init; } = 0;
        public byte Race { get; init; }
        public byte Tribe { get; init; }
        public int Gender { get; init; }
        public int CityState { get; init; }
        public int NamedayDay { get; init; }
        public int NamedayMonth { get; init; }
        public int Guardian { get; init; }
    }
}
