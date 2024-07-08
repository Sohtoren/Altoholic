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

            Lumina.Excel.GeneratedSheets.TripleTriadCard? ttc = Utils.GetTripleTriadCard(lang, id);
            if (ttc is null)
            {
                return null;
            }

            ret = new TripleTriadCard { Id = ttc.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = ttc.Name;
                    ret.GermanDescription = ttc.Description;
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = ttc.Name;
                    ret.EnglishDescription = ttc.Description;
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = ttc.Name;
                    ret.FrenchDescription = ttc.Description;
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = ttc.Name;
                    ret.JapaneseDescription = ttc.Description;
                    break;
            }

            ret.Icon = ttc.RowId + 88000;

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
