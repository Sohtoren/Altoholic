using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Armoire = Altoholic.Models.Armoire;

namespace Altoholic.Cache
{
    public class ArmoireStorage(int size = 0) : IDisposable
    {
        private readonly Dictionary<uint, Armoire> _armoire = new(size);
        /*private readonly Dictionary<uint, ArmoireCategory> _armoireCategories = new(size);
        private readonly Dictionary<uint, ArmoireSubCategory> _armoireSubCategories = new(size);*/

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<Armoire>? armoireList = Utils.GetAllArmoire(currentLocale);
            if (armoireList == null || armoireList.Count == 0)
            {
                return;
            }

            foreach (Armoire a in armoireList)
            {
                Item? i = globalCache.ItemStorage.LoadItem(currentLocale, a.ItemId);
                if (i == null)
                {
                    continue;
                }

                uint icon = i.Value.Icon;
                globalCache.IconStorage.LoadIcon(icon);
                _armoire.Add(a.Id, a);
            }

            /*List<ArmoireSubCategory>? cabinetSubCategories = Utils.GetAllArmoireSubCategories(currentLocale);
            foreach (ArmoireSubCategory cabinetSubCategory in cabinetSubCategories)
            {
                _armoireSubCategories.Add(cabinetSubCategory.Id, cabinetSubCategory);
            }*/
        }
        public bool CanBeInArmoire(uint id)
        {
            return _armoire.ContainsKey(id);
        }
        public int Count()
        {
            return _armoire.Count;
        }
        public List<uint> Get()
        {
            return _armoire.Keys.ToList();
        }

        public Armoire? GetArmoire(uint id)
        {
            return _armoire.GetValueOrDefault(id);
        }

        /*public ArmoireSubCategory? GetCategory(uint id)
        {
            return _armoireCategories.GetValueOrDefault(id);
        }
        public ArmoireSubCategory? GetSubCategory(uint id)
        {
            return _armoireSubCategories.GetValueOrDefault(id);
        }
        public List<ArmoireSubCategory> GetSubCategories()
        {
            return _armoireSubCategories.Values.ToList();
        }*/
        public void Dispose()
        {
            _armoire.Clear();
            /*_armoireCategories.Clear();
            _armoireSubCategories.Clear();*/
        }
    }
}
