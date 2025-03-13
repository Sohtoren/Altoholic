using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class HairstyleStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Hairstyle> _hairstylesAndFaces = new(size);
        private readonly Dictionary<int, List<uint>> _hairstylesPerSubRacesAndGender = new();
        private readonly Dictionary<int, List<uint>> _facesPerSubRacesAndGender = new();
        private readonly ExcelSheet<RawRow> _hairMakeType = Plugin.DataManager.GetExcelSheet<RawRow>(ClientLanguage.English, "HairMakeType");

        public void Init(GlobalCache globalCache)
        {
            List<Hairstyle>? hairstyles = Utils.GetAllHairstlyles();
            if (hairstyles == null || hairstyles.Count == 0)
            {
                return;
            }

            foreach (Hairstyle h in hairstyles)
            {
                globalCache.IconStorage.LoadIcon(h.Icon);
                _hairstylesAndFaces.Add(h.Id, h);
            }

            for (int i = 0; i <= 31; i++)
            {
                LoadHairstylesPerRaces(i);
                LoadFacepaintsPerRaces(i);
            }
        }

        //private void LoadHairstylesPerRaces(uint subRace, uint gender)
        private void LoadHairstylesPerRaces(int subRace)
        {
            //RawRow row = hairMakeType.GetRow((subRace - 1) * 2 - 1 + gender);
            RawRow row = _hairMakeType.GetRow((uint)subRace);
            // Unknown30 is the number of available hairstyles.
            byte numHairs = row.ReadUInt8Column(30);
            List<uint> hairList = new(numHairs);
            // Hairstyles can be found starting at Unknown66.
            for (int i = 0; i < numHairs; ++i)
            {
                // Hairs start at Unknown66.
                uint index = row.ReadUInt32Column(66 + i * 9);
                if (index == uint.MaxValue)
                    continue;

                Hairstyle? h = GetHairstyle(ClientLanguage.English, index);
                if(h is not { IsPurchasable: true })
                    continue;

                hairList.Add(index);
            }
            _hairstylesPerSubRacesAndGender.Add(subRace, hairList);
        }
        private void LoadFacepaintsPerRaces(int subRace)
        {
            //RawRow row = hairMakeType.GetRow((subRace - 1) * 2 - 1 + gender);
            RawRow row = _hairMakeType.GetRow((uint)subRace);
            // Unknown30 is the number of available hairstyles.
            byte numPaints = row.ReadUInt8Column(37);
            List<uint> facepaintList = new(numPaints);
            // Hairstyles can be found starting at Unknown66.
            for (int i = 0; i < numPaints; ++i)
            {
                // Hairs start at Unknown66.
                uint index = row.ReadUInt32Column(73 + i * 9);
                if (index == uint.MaxValue)
                    continue;

                Hairstyle? h = GetHairstyle(ClientLanguage.English, index);
                if(h is not { IsPurchasable: true })
                    continue;

                facepaintList.Add(index);
            }
            _facesPerSubRacesAndGender.Add(subRace, facepaintList);
        }

        public Hairstyle? GetHairstyle(ClientLanguage lang, uint id)
        {
            if (_hairstylesAndFaces.TryGetValue(id, out Hairstyle? ret))
                return ret;

            CharaMakeCustomize? hairstyle = Utils.GetHairstlyle(lang, id);
            if (hairstyle is null)
            {
                return null;
            }

            if (hairstyle.Value.HintItem.Value.Name.IsEmpty) return null;
            ret = new Hairstyle
            {
                Id = hairstyle.Value.RowId,
                Icon = hairstyle.Value.Icon,
                IsPurchasable = hairstyle.Value.IsPurchasable,
                SortKey = hairstyle.Value.Data,
                UnlockLink = hairstyle.Value.Data
            };
            if (hairstyle.Value.Data == 228)
            {
                switch (lang)
                {
                    case ClientLanguage.German:
                        ret.GermanName = "Ewigen Bundes";
                        break;
                    case ClientLanguage.English:
                        ret.EnglishName = "Eternal Bonding";
                        break;
                    case ClientLanguage.French:
                        ret.FrenchName = "Lien Éternel";
                        break;
                    case ClientLanguage.Japanese:
                        ret.JapaneseName = "エターナルバンド";
                        break;
                }
            }
            else
            {
                Item? i = Utils.GetItemFromId(lang, ret.Id);
                if (i == null) return null;
                switch (lang)
                {
                    case ClientLanguage.German:
                        ret.GermanName = i.Value.Name.ExtractText();
                        break;
                    case ClientLanguage.English:
                        ret.EnglishName = i.Value.Name.ExtractText();
                        break;
                    case ClientLanguage.French:
                        ret.FrenchName = i.Value.Name.ExtractText();
                        break;
                    case ClientLanguage.Japanese:
                        ret.JapaneseName = i.Value.Name.ExtractText();
                        break;
                }
            }

            return ret;
        }

        public void Add(uint id, Hairstyle h)
        {
            _hairstylesAndFaces.Add(id, h);
        }

        public int Count()
        {
            return _hairstylesAndFaces.Count;
        }
        public List<uint> Get()
        {
            return _hairstylesAndFaces.Keys.ToList();
        }
        public Dictionary<uint, Hairstyle> GetAll()
        {
            return _hairstylesAndFaces;
        }

        public List<uint> GetIdsFromStartIndex(int startIndex)
        {
            return _hairstylesAndFaces.Where(h => h.Value.Id >= startIndex).Select(x => x.Key).ToList();
        }
        public List<uint> GetIdsFromUnlockLink(ushort unlockLink)
        {
            return _hairstylesAndFaces.Where(h => h.Value.UnlockLink == unlockLink).Select(x => x.Key).ToList();
        }
        public void Dispose()
        {
            _hairstylesAndFaces.Clear();
            _hairstylesPerSubRacesAndGender.Clear();
            _facesPerSubRacesAndGender.Clear();
        }

        public bool IsHairstyleAvailableForRaceGender(byte tribe, int gender, uint hairstyleId)
        {
            int row = GetRowFromTribeGender(tribe, gender);
            List<uint> hlist = _hairstylesPerSubRacesAndGender.Where(h => h.Key == row).Select(h => h.Value).ToList().First();
            return hlist.Contains(hairstyleId);
        }

        public List<uint> GetAllHairstylesForTribeGender(byte tribe, int gender)
        {
            return _hairstylesPerSubRacesAndGender.Where(h => h.Key == GetRowFromTribeGender(tribe, gender)).Select(h => h.Value).ToList().First();
        }
        public bool IsFacepaintAvailableForRaceGender(byte tribe, int gender, uint hairstyleId)
        {
            int row = GetRowFromTribeGender(tribe, gender);
            List<uint> hlist = _facesPerSubRacesAndGender.Where(h => h.Key == row).Select(h => h.Value).ToList().First();
            return hlist.Contains(hairstyleId);
        }

        public List<uint> GetAllFacepaintsForTribeGender(byte tribe, int gender)
        {
            return _facesPerSubRacesAndGender.Where(h => h.Key == GetRowFromTribeGender(tribe, gender)).Select(h => h.Value).ToList().First();
        }

        private static int GetRowFromTribeGender(byte tribe, int gender)
        {
            return tribe switch
            {
                1 => (gender == 0) ? 0 : 1,
                2 => (gender == 0) ? 2 : 3,
                3 => (gender == 0) ? 4 : 5,
                4 => (gender == 0) ? 6 : 7,
                5 => (gender == 0) ? 8 : 9,
                6 => (gender == 0) ? 10 : 11,
                7 => (gender == 0) ? 12 : 13,
                8 => (gender == 0) ? 14 : 15,
                9 => (gender == 0) ? 16 : 17,
                10 => (gender == 0) ? 18 : 19,
                11 => (gender == 0) ? 20 : 21,
                12 => (gender == 0) ? 22 : 23,
                13 => (gender == 0) ? 24 : 25,
                14 => (gender == 0) ? 26 : 27,
                15 => (gender == 0) ? 28 : 29,
                16 => (gender == 0) ? 30 : 31,
                _ => 0
            };
    }
    }
}
