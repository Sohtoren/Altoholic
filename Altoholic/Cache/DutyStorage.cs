using Altoholic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class DutyStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Duty> _duties = new(size);

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
        public void Dispose()
        {
            _duties.Clear();
        }

        public Duty? GetFromTerritory(ushort e)
        {
            return _duties.Values.FirstOrDefault(d => d.TerritoryType == e);
        }
    }
}
