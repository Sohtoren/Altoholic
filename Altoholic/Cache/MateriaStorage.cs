using System;
using System.Collections.Generic;
using System.Linq;
using Materia = Altoholic.Models.Materia;

namespace Altoholic.Cache
{
    public class MateriaStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Materia> _places = new(size);

        public void Init()
        {
            List<Materia>? materia = Utils.GetAllMaterias();
            if (materia == null || materia.Count == 0)
            {
                return;
            }

            foreach (Materia m in materia)
            {
                _places.Add(m.Id, m);
            }
        }

        public uint? GetMateriaItemId(ushort id, byte grade)
        {
            if (_places.TryGetValue(id, out Materia? ret))
                return ret.Grades[grade];

            Lumina.Excel.Sheets.Materia? materia = Utils.GetMateria(id);

            return materia?.Item[grade].Value.RowId;
        }

        public Materia? GetMateria(ushort id)
        {
            if (_places.TryGetValue(id, out Materia? ret))
                return ret;

            Lumina.Excel.Sheets.Materia? m = Utils.GetMateria(id);
            if (m is null)
            {
                return null;
            }
            ret = new Materia { Id = m.Value.RowId, BaseParamId = m.Value.BaseParam.RowId, Grades = new uint[16], Values = new short[16] };
            for (int i = 0; i < 16; i++)
            {
                ret.Grades[i] = m.Value.Item[i].RowId;
                ret.Values[i] = m.Value.Value[i];
            }

            return ret;
        }
        public void Add(uint id, Materia m)
        {
            _places.Add(id, m);
        }

        public int Count()
        {
            return _places.Count;
        }
        public List<uint> Get()
        {
            return _places.Keys.ToList();
        }
        public void Dispose()
        {
            _places.Clear();
        }
    }
}
