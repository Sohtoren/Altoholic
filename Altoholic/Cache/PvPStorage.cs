using Dalamud.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;

namespace Altoholic.Cache
{

    public class PvPStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Models.PvPRank> _ranks = new(size);
        private Dictionary<uint, PvPSeries> _series = new(size);
        private Dictionary<uint, PvPSeriesLevel> _seriesLevel = new(size);
        private uint _lastSerieId;
        public void Init(ClientLanguage currentLocale)
        {
            List<Models.PvPRank>? r = Utils.GetPvPRanks(currentLocale);
            if (r == null || r.Count == 0)
            {
                return;
            }
            foreach (Models.PvPRank pvPRank in r)
            {
                _ranks.Add(pvPRank.Id, pvPRank);
            }
            
            (_series, _lastSerieId) = Utils.GetPvPSeries(currentLocale);
            _seriesLevel = Utils.GetPvPSeriesLevel(currentLocale);
        }

        public int Count()
        {
            return _ranks.Count;
        }

        public PvPSeries? GetSeries(uint id)
        {
            return _series.GetValueOrDefault(id);
        }
        public PvPSeriesLevel? GetSeriesExperience(uint id)
        {
            return _seriesLevel.GetValueOrDefault(id);
        }
        public uint GetRankExperience(uint rank)
        {
            Models.PvPRank? r = _ranks.GetValueOrDefault(rank);
            return r?.ExpRequired ?? 0;
        }
        public Models.PvPRank? GetRank(uint rank)
        {
            return _ranks.GetValueOrDefault(rank);
        }
        public Dictionary<uint, Models.PvPRank> Get()
        {
            return _ranks;
        }
        public void Dispose()
        {
            _ranks.Clear();
            _series.Clear();
            _seriesLevel.Clear();
        }

        public uint GetLastSeriesId()
        {
            return _lastSerieId;
        }
    }
}
