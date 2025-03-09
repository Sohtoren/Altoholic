using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class HairstyleStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Hairstyle> _hairstyles = new(size);

        public void Init(GlobalCache globalCache)
        {
            List<Hairstyle>? hairstyles = Utils.GetAllHairstlyles();
            if (hairstyles == null || hairstyles.Count == 0)
            {
                return;
            }

            foreach (Hairstyle h in hairstyles)
            {
                globalCache.IconStorage.LoadIcon(h.Icon);
                _hairstyles.Add(h.Id, h);
            }
        }

        public Hairstyle? GetHairstyle(ClientLanguage lang, uint id)
        {
            if (_hairstyles.TryGetValue(id, out Hairstyle? ret))
                return ret;

            CharaMakeCustomize? hairstyle = Utils.GetHairstlyle(lang, id);
            if (hairstyle is null)
            {
                return null;
            }

            if (hairstyle.Value.HintItem.Value.Name.IsEmpty) return null;
            ret = new Hairstyle
            {
                Id = hairstyle.Value.RowId,
                Icon = hairstyle.Value.Icon,
                IsPurchasable = hairstyle.Value.IsPurchasable,
                SortKey = hairstyle.Value.Data,
                UnlockLink = hairstyle.Value.Data
            };
            if (hairstyle.Value.Data == 228)
            {
                switch (lang)
                {
                    case ClientLanguage.German:
                        ret.GermanName = "Ewigen Bundes";
                        break;
                    case ClientLanguage.English:
                        ret.EnglishName = "Eternal Bonding";
                        break;
                    case ClientLanguage.French:
                        ret.FrenchName = "Lien Éternel";
                        break;
                    case ClientLanguage.Japanese:
                        ret.JapaneseName = "エターナルバンド";
                        break;
                }
            }
            else
            {
                Item? i = Utils.GetItemFromId(lang, ret.Id);
                if (i == null) return null;
                switch (lang)
                {
                    case ClientLanguage.German:
                        ret.GermanName = i.Value.Name.ExtractText();
                        break;
                    case ClientLanguage.English:
                        ret.EnglishName = i.Value.Name.ExtractText();
                        break;
                    case ClientLanguage.French:
                        ret.FrenchName = i.Value.Name.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        ret.JapaneseName = i.Value.Name.ExtractText();
                        break;
                }
            }

            return ret;
        }

        public void Add(uint id, Hairstyle h)
        {
            _hairstyles.Add(id, h);
        }

        public int Count()
        {
            return _hairstyles.Count;
        }
        public List<uint> Get()
        {
            return _hairstyles.Keys.ToList();
        }
        public Dictionary<uint, Hairstyle> GetAll()
        {
            return _hairstyles;
        }

        public List<uint> GetIdsFromUnlockLink(ushort unlockLink)
        {
            return _hairstyles.Where(h => h.Value.UnlockLink == unlockLink).Select(x => x.Key).ToList();
        }
        public void Dispose()
        {
            _hairstyles.Clear();
        }
    }
}
