namespace Altoholic.Models
{
    public class Profile
    {
        public string Title { get; set; } = string.Empty;
        public bool TitleIsPrefix { get; set; } = false;
        public int GrandCompany { get; set; } = 0;
        public int GrandCompanyRank { get; set; } = 0;
        public byte Race { get; set; }
        public byte Tribe { get; set; }
        public int Gender { get; set; }
        public int CityState { get; set; }
        public int NamedayDay { get; set; }
        public int NamedayMonth { get; set; }
        public int Guardian { get; set; }
    }
}
