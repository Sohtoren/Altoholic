using Dalamud.Game;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using Item = Lumina.Excel.GeneratedSheets.Item;

namespace Altoholic.Cache
{
    public class ItemItemLevel
    {
        public Item Item { get; set; } = null!;
        public ItemLevel? ItemLevel { get; set; }
    }
    public class ItemStorage(int size = 0)
    {
        private readonly Dictionary<uint, ItemItemLevel> _items = new(size);
        private readonly Dictionary<uint, EventItem> _eventItems = new(size);
        private List<uint> _armoireItems = [];

        public void Init()
        {
            _armoireItems = Utils.GetArmoireIds();
            Plugin.Log.Debug($"ItemStorage Init() {_armoireItems.Count} items");
            Plugin.Log.Debug($"ItemStorage Init() contains? : {_armoireItems.Contains(41678)}");
        }

        public Item? LoadItem(ClientLanguage currentLocale, uint id)
        {
            if (_items.TryGetValue(id, out ItemItemLevel? ret))
                return ret.Item;


            Item? dbItem = Utils.GetItemFromId(currentLocale, id);
            if (dbItem == null) return null;
            ret = new ItemItemLevel
            {
                Item = dbItem,
                ItemLevel = null
            };
            _items[id] = ret;
            return ret.Item;
        }

        public ItemItemLevel? LoadItemWithItemLevel(ClientLanguage currentLocale, uint id)
        {
            if (_items.TryGetValue(id, out ItemItemLevel? ret))
                return ret;

            Item? dbItem = Utils.GetItemFromId(currentLocale, id);
            if(dbItem == null) return null;
            ret = new ItemItemLevel
            {
                Item = dbItem,
                ItemLevel = Utils.GetItemLevelFromId(dbItem.LevelItem.Row)!
            };
            _items[id] = ret;
            return ret;
        }

        /*public List<Item>? GetItemsFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return _items.Where(i => i.Value.item.Name == name).ToList();
        }*/

        public Item? LoadItemFromName(ClientLanguage currentLocale, string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            KeyValuePair<uint, ItemItemLevel> item = _items.FirstOrDefault(i => i.Value.Item.Name == name);
            if (item.Value is not null)
            {
                return item.Value.Item;
            }

            Item? dbItem = Utils.GetItemFromName(currentLocale, name);
            if (dbItem == null) return null;
            ItemItemLevel ret = new()
            {
                Item = dbItem,
                ItemLevel = null
            };
            _items[dbItem.RowId] = ret;
            return ret.Item;
        }

        public EventItem? LoadEventItem(ClientLanguage currentLocale, uint id)
        {
            if (_eventItems.TryGetValue(id, out EventItem? ret))
                return ret;

            EventItem? item = Utils.GetEventItemFromId(currentLocale, id);
            if (item == null) return null;
            _eventItems[id] = item;
            return item;
        }

        public bool CanBeInArmoire(uint id)
        {
            return _armoireItems.Contains(id);
        }
        
        public void Dispose()
        {
            _items.Clear();
            _eventItems.Clear();
        }
    }
}
