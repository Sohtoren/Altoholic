using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using Glasses = Altoholic.Models.Glasses;

namespace Altoholic.Cache
{
    public class GlassesStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Glasses> _glasses = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<Glasses>? glasses = Utils.GetAllGlasses(currentLocale);
            if (glasses == null || glasses.Count == 0)
            {
                return;
            }

            foreach (Glasses glass in glasses.Where(g =>
                         g.Id is 1 or 13 or 25 or 37 or 49 or 61 or 73 or 85 or 97 or 109 or 121 or 133 or 145 or 157
                             or 169 or 181 or 193 or 205 or 217 or 229 or 241 or 253 or 265 or 277 or 289 or 301 or 313 or 325))
            {
                globalCache.IconStorage.LoadIcon(glass.Icon);
                _glasses.Add(glass.Id, glass);
            }
        }

        public Glasses? GetGlasses(ClientLanguage lang, uint id)
        {
            if (_glasses.TryGetValue(id, out Glasses? ret))
                return ret;

            Lumina.Excel.Sheets.Glasses? glasses = Utils.GetGlasses(lang, id);
            if (glasses is null)
            {
                return null;
            }

            ret = new Glasses { Id = glasses.Value.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = glasses.Value.Singular.ExtractText();
                    ret.GermanDescription = glasses.Value.Description.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = glasses.Value.Singular.ExtractText();
                    ret.EnglishDescription = glasses.Value.Description.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = glasses.Value.Singular.ExtractText();
                    ret.FrenchDescription = glasses.Value.Description.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = glasses.Value.Singular.ExtractText();
                    ret.JapaneseDescription = glasses.Value.Description.ExtractText();
                    break;
            }

            ret.Icon = (uint)glasses.Value.Icon;

            return ret;
        }

        public void Add(uint id, Glasses m)
        {
            _glasses.Add(id, m);
        }

        public int Count()
        {
            return _glasses.Count;
        }
        public List<uint> Get()
        {
            return _glasses.Keys.ToList();
        }
        public void Dispose()
        {
            _glasses.Clear();
        }
    }
}
