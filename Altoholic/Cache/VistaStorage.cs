using Altoholic.Models;
using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class VistaStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Vista> _vistas = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<Vista>? vistas = Utils.GetAllVista(currentLocale);
            if (vistas == null || vistas.Count == 0)
            {
                return;
            }

            foreach (Vista s in vistas)
            {
                globalCache.IconStorage.LoadIcon((uint)s.IconList);
                globalCache.IconStorage.LoadIcon((uint)s.IconDiscovered);
                globalCache.IconStorage.LoadIcon((uint)s.IconUndiscovered);
                _vistas.Add(s.Id, s);
            }
        }

        public Vista? GetVista(ClientLanguage lang, uint id)
        {
            if (_vistas.TryGetValue(id, out Vista? ret))
                return ret;

            Lumina.Excel.Sheets.Adventure? v = Utils.GetVista(lang, id);
            if (v is null)
            {
                return null;
            }

            ret = new Vista
            {
                Id = v.Value.RowId,
                LevelId = v.Value.Level.RowId,
                MinLevel = v.Value.MinLevel,
                MaxLevel = v.Value.MaxLevel,
                Emote = v.Value.Emote.RowId,
                PlaceNameId = v.Value.RowId,
                IconList = v.Value.IconList,
                IconDiscovered = v.Value.IconDiscovered,
                IconUndiscovered = v.Value.IconUndiscovered,
                IsInitial = v.Value.IsInitial,
            };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = v.Value.Name.ExtractText();
                    ret.GermanName = v.Value.Impression.ExtractText();
                    ret.GermanName = v.Value.Description.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = v.Value.Name.ExtractText();
                    ret.EnglishName = v.Value.Impression.ExtractText();
                    ret.EnglishName = v.Value.Description.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = v.Value.Name.ExtractText();
                    ret.FrenchName = v.Value.Impression.ExtractText();
                    ret.FrenchName = v.Value.Description.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = v.Value.Name.ExtractText();
                    ret.JapaneseName = v.Value.Impression.ExtractText();
                    ret.JapaneseName = v.Value.Description.ExtractText();
                    break;
            }

            return ret;
        }

        public void Add(uint id, Vista or)
        {
            _vistas.Add(id, or);
        }

        public int Count()
        {
            return _vistas.Count;
        }
        public List<uint> Get()
        {
            return _vistas.Keys.ToList();
        }
        public void Dispose()
        {
            _vistas.Clear();
        }
    }
}
