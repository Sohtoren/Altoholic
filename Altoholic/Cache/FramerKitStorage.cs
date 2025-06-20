using Altoholic.Models;
using Dalamud.Game;
using System.Collections.Generic;
using System.Linq;
using Item = Lumina.Excel.Sheets.Item;

namespace Altoholic.Cache
{
    public class FramerKitStorage(int size = 0)
    {
        private readonly Dictionary<uint, FramerKit> _framerKits = new(size);

        public void Init(GlobalCache globalCache)
        {
            IEnumerable<Item>? items = Utils.GetItemsFromItemAction(ClientLanguage.English, 2234);
            using IEnumerator<Item>? itms = items?.GetEnumerator();
            if (itms is null) return;
            while (itms.MoveNext())
            {
                Item itm = itms.Current;
                List<ClientLanguage> langs =
                    [ClientLanguage.German, ClientLanguage.English, ClientLanguage.French, ClientLanguage.Japanese];
                uint fkId = itm.AdditionalData.RowId;
                FramerKit fk = new() { Id = fkId, ItemId = itm.RowId };
                foreach (ClientLanguage l in langs)
                {
                    Item? item = Utils.GetItemFromId(l, itm.RowId);
                    if (item == null) continue;
                    switch (l)
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
                }
                fk.Icon = itm.Icon;
                globalCache.IconStorage.LoadIcon(fk.Icon);
                Plugin.Log.Debug($"itemId: {itm.RowId}, AdditionalData: {itm.AdditionalData.RowId}");
                _framerKits.Add(fkId, fk);
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
