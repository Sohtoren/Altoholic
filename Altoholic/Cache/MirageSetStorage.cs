using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class MirageSetStorage(int size = 120) : IDisposable
    {
        private Dictionary<uint, HashSet<uint>> _mirageSetLookup = new(size);
        private Dictionary<uint, HashSet<uint>> _mirageSetItemLookup = new(size);

        public void Init(GlobalCache globalCache)
        {
            _mirageSetLookup = Utils.GetMirageStoreSetItems().ToDictionary(c => c.RowId, c => new List<uint>()
            {
                c.MainHand.RowId, c.OffHand.RowId, c.Head.RowId, c.Body.RowId, c.Hands.RowId, c.Legs.RowId, c.Feet.RowId, c.Earrings.RowId,
                c.Necklace.RowId, c.Bracelets.RowId, c.Ring.RowId
            }.Where(u => u != 0).Distinct().ToHashSet());

            _mirageSetItemLookup = new Dictionary<uint, HashSet<uint>>();

            foreach (KeyValuePair<uint, HashSet<uint>> set in _mirageSetLookup)
            {
                foreach (uint setItem in set.Value)
                {
                    _mirageSetItemLookup.TryAdd(setItem, []);
                    _mirageSetItemLookup[setItem].Add(set.Key);
                }
            }
        }

        public bool MirageSetLookup(uint id)
        {
            return _mirageSetLookup.TryGetValue(id, out HashSet<uint>? _);
        }
        public HashSet<uint>? GetMirageSetLookup(uint id)
        {
            return _mirageSetLookup.GetValueOrDefault(id);
        }
        public HashSet<uint>? GetMirageSetItemLookup(uint id)
        {
            return _mirageSetItemLookup.GetValueOrDefault(id);
        }
        public void Dispose()
        {
            _mirageSetLookup.Clear();
            _mirageSetItemLookup.Clear();
        }
    }
}
