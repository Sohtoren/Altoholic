using Altoholic.Models;
using Dalamud.Game;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Item = Lumina.Excel.Sheets.Item;

namespace Altoholic.Cache
{
    public class FramerKitStorage(int size = 0)
    {
        private readonly Dictionary<uint, FramerKit> _framerKits = new(size);
        private readonly uint[] _framerKitsIds = [37247, 37248, 37249, 37250, 37251, 37252, 37253, 37254, 37255, 37256, 37257, 37258, 37259, 37260, 37261, 37262, 37263, 37264, 37265, 37266, 37267, 37269, 37270, 37271, 37272, 37273, 37274, 37275, 37276, 37277, 38348, 38349, 38353, 38354, 38355, 38356, 38357, 38358, 38359, 38360, 38361, 38365, 38366, 38367, 38368, 38369, 38370, 38371, 38372, 38373, 38465, 38466, 39439, 39440, 39441, 39442, 39443, 39444, 39445, 39446, 39447, 39451, 39452, 39453, 39454, 39455, 39456, 39457, 39458, 39459, 39570, 39571, 39572, 39573, 39574, 39575, 39576, 39577, 39578, 39579, 39580, 40480, 40481, 40482, 40483, 40484, 40485, 40486, 40487, 40488, 40489, 40490, 40491, 40492, 40493, 40494, 40495, 40496, 40497, 40498, 40499, 40500, 40501, 40502, 40503, 40504, 40505, 40506, 40507, 40508, 40509, 41322, 41323, 41324, 41325, 41326, 41327, 41328, 41329, 41330, 41331, 41332, 41333, 41334, 41335, 41336, 41337, 41338, 41339, 41340, 41341, 41342, 41343, 41344, 41345, 41346, 41347, 41348, 41349, 41350, 41351, 41352, 41353, 41354, 41355, 41356, 41357, 41367, 41368, 41369, 41370, 41371, 41372, 41373, 41374, 41375, 41376, 41377, 41378, 41379, 41797, 43565, 43566, 43953, 43954, 43955, 43956, 43957, 43958, 43959, 43960, 44292, 44293, 44294, 44350, 44351, 44944, 44945, 44946, 44947, 44948, 45079, 45080, 45081, 45082, 45083, 45084, 45085, 45086, 45087, 45088, 45089, 45090];

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            foreach (uint framerKitsId in _framerKitsIds)
            {
                Item? item = Utils.GetItemFromId(currentLocale, framerKitsId);
                if (item == null)
                {
                    continue;
                }
                FramerKit fk = new() { Id = item.Value.AdditionalData.RowId, ItemId = item.Value.RowId };
                switch (currentLocale)
                {
                    case ClientLanguage.German:
                        fk.GermanName = item.Value.Name.ExtractText();
                        break;
                    case ClientLanguage.English:
                        fk.EnglishName = item.Value.Name.ExtractText();
                        break;
                    case ClientLanguage.French:
                        fk.FrenchName = item.Value.Name.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        fk.JapaneseName = item.Value.Name.ExtractText();
                        break;
                }
                fk.Icon = item.Value.Icon;
                globalCache.IconStorage.LoadIcon(fk.Icon);
                _framerKits.Add(item.Value.AdditionalData.RowId, fk);
            }
        }

        public FramerKit? LoadItem(ClientLanguage currentLocale, uint id)
        {
            if (_framerKits.TryGetValue(id, out FramerKit? ret))
                return ret;

            Item? dbItem = Utils.GetItemFromId(currentLocale, id);
            if (dbItem == null) return null;
            ret = new FramerKit { Id = dbItem.Value.RowId };
            switch (currentLocale)
            {
                case ClientLanguage.German:
                    ret.GermanName = dbItem.Value.Name.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = dbItem.Value.Name.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = dbItem.Value.Name.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = dbItem.Value.Name.ExtractText();
                    break;
            }
            ret.Icon = dbItem.Value.Icon;
            _framerKits[id] = ret;
            return ret;
        }

        public uint? GetFramerKitIdFromItemId(uint itemId)
        {
            if (itemId == 0) return null;

            KeyValuePair<uint, FramerKit> fk = _framerKits.FirstOrDefault(i => i.Value.ItemId == itemId);
            return fk.Value?.Id;
        }

        public int Count()
        {
            return _framerKits.Count;
        }

        public List<uint> Get()
        {
            return _framerKits.Keys.ToList();
        }

        public void Dispose()
        {
            _framerKits.Clear();
        }
    }
}
