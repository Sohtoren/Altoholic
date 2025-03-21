namespace Altoholic.Models
{
    /*public class ArmoireSubCategory
    {
        public uint Id { get; set; }
        public string GermanName { get; set; }
        public string EnglishName { get; set; }
        public string FrenchName { get; set; }
        public string JapaneseName { get; set; }
        public int MenuOrder { get; set; }
    }
    public class ArmoireCategory
    {
        public uint Id { get; set; }
        public string GermanName { get; set; }
        public string EnglishName { get; set; }
        public string FrenchName { get; set; }
        public string JapaneseName { get; set; }
        public int MenuOrder { get; set; }
        public int HideOrder { get; set; }
        public uint Icon { get;set }
    }*/
    public class Armoire
    {
        public uint Id { get; set; }
        public uint ItemId { get; set; }
        public int Order { get; set; }
        public uint ArmoireCategory { get; set; }
        public uint ArmoireSubcategory { get; set; }
    }
}
