using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using BaseParam = Altoholic.Models.BaseParam;

namespace Altoholic.Cache
{
    public class BaseParamStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, BaseParam> _params = new(size);

        public void Init(ClientLanguage currentLocale)
        {
            List<BaseParam>? baseParam = Utils.GetAllBaseParams(currentLocale);
            if (baseParam == null || baseParam.Count == 0)
            {
                return;
            }

            foreach (BaseParam place in baseParam)
            {
                _params.Add(place.Id, place);
            }
        }

        public BaseParam? GetBaseParam(ClientLanguage lang, uint id)
        {
            if (_params.TryGetValue(id, out BaseParam? ret))
                return ret;

            Lumina.Excel.Sheets.BaseParam? baseParam = Utils.GetBaseParam(lang, id);
            if (baseParam is null)
            {
                return null;
            }

            ret = new()
            {
                Id = baseParam.Value.RowId,
                OneHandWeaponPercent = baseParam.Value.OneHandWeaponPercent,
                OffHandPercent = baseParam.Value.OffHandPercent,
                HeadPercent = baseParam.Value.HeadPercent,
                ChestPercent = baseParam.Value.ChestPercent,
                HandsPercent = baseParam.Value.HandsPercent,
                WaistPercent = baseParam.Value.WaistPercent,
                LegsPercent = baseParam.Value.LegsPercent,
                FeetPercent = baseParam.Value.FeetPercent,
                EarringPercent = baseParam.Value.EarringPercent,
                NecklacePercent = baseParam.Value.NecklacePercent,
                BraceletPercent = baseParam.Value.BraceletPercent,
                RingPercent = baseParam.Value.RingPercent,
                TwoHandWeaponPercent = baseParam.Value.TwoHandWeaponPercent,
                UnderArmorPercent = baseParam.Value.UnderArmorPercent,
                ChestHeadPercent = baseParam.Value.ChestHeadPercent,
                ChestHeadLegsFeetPercent = baseParam.Value.ChestHeadLegsFeetPercent,
                Unknown0 = baseParam.Value.Unknown0,
                LegsFeetPercent = baseParam.Value.LegsFeetPercent,
                HeadChestHandsLegsFeetPercent = baseParam.Value.HeadChestHandsLegsFeetPercent,
                ChestLegsGlovesPercent = baseParam.Value.ChestLegsGlovesPercent,
                ChestLegsFeetPercent = baseParam.Value.ChestLegsFeetPercent,
                Unknown1 = baseParam.Value.Unknown1,
                OrderPriority = baseParam.Value.OrderPriority,
                MeldParam = [.. baseParam.Value.MeldParam],
                PacketIndex = baseParam.Value.PacketIndex,
                Unknown2 = baseParam.Value.Unknown2
            };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = baseParam.Value.Name.ExtractText();
                    ret.GermanDescription = baseParam.Value.Description.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = baseParam.Value.Name.ExtractText();
                    ret.EnglishDescription = baseParam.Value.Description.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = baseParam.Value.Name.ExtractText();
                    ret.FrenchDescription = baseParam.Value.Description.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = baseParam.Value.Name.ExtractText();
                    ret.JapaneseDescription = baseParam.Value.Description.ExtractText();
                    break;
            }

            return ret;
        }
        public void Add(uint id, BaseParam m)
        {
            _params.Add(id, m);
        }

        public int Count()
        {
            return _params.Count;
        }
        public List<uint> Get()
        {
            return _params.Keys.ToList();
        }
        public void Dispose()
        {
            _params.Clear();
        }
    }
}
