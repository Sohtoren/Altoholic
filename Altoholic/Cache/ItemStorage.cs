using Dalamud;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

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

        /*public Item? this[uint id]
            => LoadItem(id);

        public Item? this[int id]
            => LoadItem((uint)id);*/

        public Item? LoadItem(ClientLanguage currentLocale, uint id)
        {
            if (_items.TryGetValue(id, out ItemItemLevel? ret))
                return ret.Item;


            Item? dbItem = Utils.GetItemFromId(currentLocale, id);
            if (dbItem == null) return null;
            ret = new ItemItemLevel
            {
                Item = dbItem,
                ItemLevel = Utils.GetItemLevelFromId(dbItem.LevelItem.Row)!
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

        public EventItem? LoadEventItem(ClientLanguage currentLocale, uint id)
        {
            if (_eventItems.TryGetValue(id, out EventItem? ret))
                return ret;

            EventItem? item = Utils.GetEventItemFromId(currentLocale, id);
            if (item == null) return null;
            _eventItems[id] = item;
            return item;
        }

        public void Dispose()
        {
            _items.Clear();
            _eventItems.Clear();
        }
    }
}
