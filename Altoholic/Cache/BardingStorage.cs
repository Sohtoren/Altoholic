using Altoholic.Models;
using Dalamud;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class BardingStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Barding> _bardings = new(size);

        public void Init(ClientLanguage currentLocale)
        {
            List<Barding>? bardings = Utils.GetAllBardings(currentLocale);
            if (bardings == null || bardings.Count == 0)
            {
                return;
            }

            foreach (Barding b in bardings)
            {
                _bardings.Add(b.Id, b);
            }
        }

        public Barding? GetBarding(ClientLanguage lang, uint id)
        {
            if (_bardings.TryGetValue(id, out Barding? ret))
                return ret;

            Lumina.Excel.GeneratedSheets.BuddyEquip? b = Utils.GetBarding(lang, id);
            if (b is null)
            {
                return null;
            }

            ret = new Barding { Id = b.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = b.Name;
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = b.Name;
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = b.Name;
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = b.Name;
                    break;
            }

            ret.IconHead = b.IconHead;
            ret.IconBody = b.IconBody;
            ret.IconLegs = b.IconLegs;

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
