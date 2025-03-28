using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using PlaceName = Altoholic.Models.PlaceName;

namespace Altoholic.Cache
{
    public class PlaceNameStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, PlaceName> _places = new(size);

        public void Init(GlobalCache globalCache)
        {
            List<PlaceName>? placeName = Utils.GetAllPlaceName();
            if (placeName == null || placeName.Count == 0)
            {
                return;
            }

            foreach (PlaceName place in placeName)
            {
                _places.Add(place.Id, place);
            }
        }

        public PlaceName? GetPlaceName(ClientLanguage lang, uint id)
        {
            if (_places.TryGetValue(id, out PlaceName? ret))
                return ret;

            Lumina.Excel.Sheets.PlaceName? placeName = Utils.GetPlaceName(lang, id);
            if (placeName is null)
            {
                return null;
            }

            ret = new PlaceName { Id = placeName.Value.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = placeName.Value.Name.ExtractText();
                    ret.GermanNoParticle = placeName.Value.NameNoArticle.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = placeName.Value.Name.ExtractText();
                    ret.EnglishNoParticle = placeName.Value.NameNoArticle.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = placeName.Value.Name.ExtractText();
                    ret.FrenchNoParticle = placeName.Value.NameNoArticle.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = placeName.Value.Name.ExtractText();
                    ret.JapaneseNoParticle = placeName.Value.NameNoArticle.ExtractText();
                    break;
            }

            return ret;
        }
        
        public string GetPlaceNameOnly(ClientLanguage lang, uint id)
        {
            if (_places.TryGetValue(id, out PlaceName? ret))
                return lang switch
                {
                    ClientLanguage.German => ret.GermanName,
                    ClientLanguage.English => ret.EnglishName,
                    ClientLanguage.French => ret.FrenchName,
                    ClientLanguage.Japanese => ret.JapaneseName,
                    _ => ret.EnglishName
                };

            Lumina.Excel.Sheets.PlaceName? placeName = Utils.GetPlaceName(lang, id);
            if (placeName is null)
            {
                return string.Empty;
            }

            ret = new PlaceName { Id = placeName.Value.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = placeName.Value.Name.ExtractText();
                    ret.GermanNoParticle = placeName.Value.NameNoArticle.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = placeName.Value.Name.ExtractText();
                    ret.EnglishNoParticle = placeName.Value.NameNoArticle.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = placeName.Value.Name.ExtractText();
                    ret.FrenchNoParticle = placeName.Value.NameNoArticle.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = placeName.Value.Name.ExtractText();
                    ret.JapaneseNoParticle = placeName.Value.NameNoArticle.ExtractText();
                    break;
            }

            return lang switch
            {
                ClientLanguage.German => ret.GermanName,
                ClientLanguage.English => ret.EnglishName,
                ClientLanguage.French => ret.FrenchName,
                ClientLanguage.Japanese => ret.JapaneseName,
                _ => ret.EnglishName
            };
        }

        public void Add(uint id, PlaceName m)
        {
            _places.Add(id, m);
        }

        public int Count()
        {
            return _places.Count;
        }
        public List<uint> Get()
        {
            return _places.Keys.ToList();
        }
        public void Dispose()
        {
            _places.Clear();
        }
    }
}
