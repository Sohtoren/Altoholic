using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class BardingStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Barding> _bardings = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<Barding>? bardings = Utils.GetAllBardings(currentLocale);
            if (bardings.Count == 0)
            {
                return;
            }

            foreach (Barding b in bardings)
            {
                globalCache.IconStorage.LoadIcon(b.Icon);
                _bardings.Add(b.Id, b);
            }
        }

        public Barding? GetBarding(ClientLanguage lang, uint id)
        {
            if (_bardings.TryGetValue(id, out Barding? ret))
                return ret;

            BuddyEquip? b = Utils.GetBarding(lang, id);
            if (b is null)
            {
                return null;
            }

            ret = new Barding { Id = b.Value.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = b.Value.Name.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = b.Value.Name.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = b.Value.Name.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = b.Value.Name.ExtractText();
                    break;
            }

            ret.IconHead = b.Value.IconHead;
            ret.IconBody = b.Value.IconBody;
            ret.IconLegs = b.Value.IconLegs;

            return ret;
        }

        public void Add(uint id, Barding b)
        {
            _bardings.Add(id, b);
        }

        public int Count()
        {
            return _bardings.Count;
        }
        public List<uint> Get()
        {
            return _bardings.Keys.ToList();
        }
        public void Dispose()
        {
            _bardings.Clear();
        }
    }
}
