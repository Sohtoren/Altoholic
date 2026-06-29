using Altoholic.Models;
using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Excel.Sheets;

namespace Altoholic.Cache
{
    public class CustomDeliveryStorage(int size = 20) : IDisposable
    {
        private readonly Dictionary<uint, CustomDelivery> _customDeliveryNPC = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<CustomDelivery>? customDeliveryNPCs = Helpers.CustomDelivery.GetAllNPC(currentLocale);
            if (customDeliveryNPCs == null || customDeliveryNPCs.Count == 0)
            {
                return;
            }

            foreach (CustomDelivery customDeliveryNPC in customDeliveryNPCs)
            {
                globalCache.IconStorage.LoadIcon((uint)customDeliveryNPC.Icon);
                _customDeliveryNPC.Add(customDeliveryNPC.Id, customDeliveryNPC);
            }
        }

        public CustomDelivery? GetCustomDeliveryNPC(ClientLanguage lang, uint id)
        {
            if (_customDeliveryNPC.TryGetValue(id, out CustomDelivery? ret))
                return ret;

            CustomDelivery? c = Helpers.CustomDelivery.GetNPC(lang, id);
            if (c is null)
            {
                return null;
            }

            ret = c;

            return ret;
        }

        public List<uint> Get()
        {
            return _customDeliveryNPC.Keys.ToList();
        }
        public int Count()
        {
            return _customDeliveryNPC.Count;
        }
        public void Dispose()
        {
            _customDeliveryNPC.Clear();
        }
    }
}
