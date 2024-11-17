using Altoholic.Models;
using Dalamud.Game;
using BeastReputationRank = Lumina.Excel.Sheets.BeastReputationRank;
using BeastTribe = Lumina.Excel.Sheets.BeastTribe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class BeastTribesStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, BeastTribes> _beastTribes = new(size);
        private readonly Dictionary<uint, BeastReputationRank> _beastTribeRanks = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<BeastTribes>? beastTribes = Utils.GetAllBeastTribes(currentLocale);
            if (beastTribes == null || beastTribes.Count == 0)
            {
                return;
            }

            foreach (BeastTribes beastTribe in beastTribes)
            {
                globalCache.IconStorage.LoadIcon(beastTribe.Icon);
                _beastTribes.Add(beastTribe.Id, beastTribe);
            }

            List<BeastReputationRank>? beastReputationRanks = Utils.GetBeastReputationRanks(currentLocale);
            if (beastReputationRanks == null || beastReputationRanks.Count == 0)
            {
                return;
            }

            foreach (BeastReputationRank beastReputationRank in beastReputationRanks)
            {
                _beastTribeRanks.Add(beastReputationRank.RowId, beastReputationRank);
            }
        }

        public BeastTribes? GetBeastTribe(ClientLanguage lang, uint id)
        {
            if (_beastTribes.TryGetValue(id, out BeastTribes? ret))
                return ret;

            BeastTribe? bt = Utils.GetBeastTribe(lang, id);
            if (bt is null)
            {
                return null;
            }

            BeastTribes b = new() { Id = bt.Value.RowId, Icon = bt.Value.Icon, MaxRank = bt.Value.MaxRank, DisplayOrder = bt.Value.DisplayOrder };
            switch (lang)
            {
                case ClientLanguage.German:
                    b.GermanName = bt.Value.Name.ExtractText();
                    break;
                case ClientLanguage.English:
                    b.EnglishName = bt.Value.Name.ExtractText();
                    break;
                case ClientLanguage.French:
                    b.FrenchName = bt.Value.Name.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    b.JapaneseName = bt.Value.Name.ExtractText();
                    break;
            }

            return b;
        }
        public BeastReputationRank? GetRank(ClientLanguage lang, uint id)
        {
            return _beastTribeRanks.TryGetValue(id, out BeastReputationRank ret) ? ret : Utils.GetBeastReputationRank(lang, id);
        }

        public List<uint> Get()
        {
            return _beastTribes.Keys.ToList();
        }
        public int Count()
        {
            return _beastTribes.Count;
        }
        public List<uint> GetRanks()
        {
            return _beastTribeRanks.Keys.ToList();
        }
        public void Dispose()
        {
            _beastTribes.Clear();
        }
    }
}
