using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using TripleTriadCard = Altoholic.Models.TripleTriadCard;

namespace Altoholic.Cache
{
    public class TripleTriadCardStorage(int size = 0) : IDisposable
    {
        private readonly Dictionary<uint, TripleTriadCard> _tripleTriadCard = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<TripleTriadCard>? ttc = Utils.GetAllTripletriadcards(currentLocale);
            if (ttc == null || ttc.Count == 0)
            {
                return;
            }

            foreach (TripleTriadCard tt in ttc)
            {
                globalCache.IconStorage.LoadIcon(tt.Icon);
                _tripleTriadCard.Add(tt.Id, tt);
            }
        }

        public TripleTriadCard? GetTripleTriadCard(ClientLanguage lang, uint id)
        {
            if (_tripleTriadCard.TryGetValue(id, out TripleTriadCard? ret))
                return ret;

            Lumina.Excel.Sheets.TripleTriadCard? ttc = Utils.GetTripleTriadCard(lang, id);
            if (ttc is null)
            {
                return null;
            }

            ret = new TripleTriadCard { Id = ttc.Value.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = ttc.Value.Name.ExtractText();
                    ret.GermanDescription = ttc.Value.Description.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = ttc.Value.Name.ExtractText();
                    ret.EnglishDescription = ttc.Value.Description.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = ttc.Value.Name.ExtractText();
                    ret.FrenchDescription = ttc.Value.Description.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = ttc.Value.Name.ExtractText();
                    ret.JapaneseDescription = ttc.Value.Description.ExtractText();
                    break;
            }

            ret.Icon = ttc.Value.RowId + 88000;

            return ret;
        }

        public void Add(uint id, TripleTriadCard ttc)
        {
            _tripleTriadCard.Add(id, ttc);
        }

        public int Count()
        {
            return _tripleTriadCard.Count;
        }
        public List<uint> Get()
        {
            return _tripleTriadCard.Keys.ToList();
        }
        public void Dispose()
        {
            _tripleTriadCard.Clear();
        }
    }
}
