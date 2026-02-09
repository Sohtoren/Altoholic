using Altoholic.Models;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class DutyStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Duty> _duties = new(size);
        private readonly Dictionary<uint, Roulette> _roulettes = new(size);

        public void Init(GlobalCache globalCache)
        {
            List<Duty>? duties = Utils.GetDutyList();
            if (duties == null || duties.Count == 0)
            {
                return;
            }

            foreach (Duty d in duties)
            {
                if (d.Icon != 0)
                {
                    globalCache.IconStorage.LoadIcon(d.Icon);
                }

                _duties.Add(d.Id, d);
            }

            List<Roulette>? roulettes = Utils.GetRouletteList();
            if (roulettes == null || roulettes.Count == 0)
            {
                return;
            }

            foreach (Roulette r in roulettes)
            {
                if (r.Icon != 0)
                {
                    globalCache.IconStorage.LoadIcon(r.Icon);
                }

                _roulettes.Add(r.Id, r);
            }
        }

        public Duty? LoadDuty(uint id)
        {
            if (_duties.TryGetValue(id, out Duty? ret))
                return ret;

            ret = Utils.GetDuty(id);
            return ret;
        }

        public void Add(uint id, Duty q)
        {
            _duties.Add(id, q);
        }

        public int Count()
        {
            return _duties.Count;
        }
        public List<uint> Get()
        {
            return _duties.Keys.ToList();
        }

        public List<Duty> GetAll()
        {
            return _duties.Values.ToList();
        }
        public List<Roulette> GetAllRoulettes()
        {
            return _roulettes.Values.ToList();
        }
        public void Dispose()
        {
            _duties.Clear();
            _roulettes.Clear();
        }

        public Duty? GetFromTerritory(ushort e)
        {
            return _duties.Values.FirstOrDefault(d => d.TerritoryType == e);
        }
    }
}
