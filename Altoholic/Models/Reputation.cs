using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altoholic.Models
{
    public class Reputation
    {
        public int commendation { get; set; }
        public Rep arr_amalj_aa { get; set; } = null!;
        public Rep arr_sylph { get; set; } = null!;
        public Rep arr_kobold { get; set; } = null!;
        public Rep arr_sahagin { get; set; } = null!;
        public Rep arr_ixali { get; set; } = null!;
        public Rep hw_vanu_vanu { get; set; } = null!;
        public Rep hw_vath { get; set; } = null!;
        public Rep hw_moogle { get; set; } = null!;
        public Rep sb_kojin { get; set; } = null!;
        public Rep sb_ananta { get; set; } = null!;
        public Rep sb_namazu { get; set; } = null!;
        public Rep shb_pixie { get; set; } = null!;
        public Rep shb_qitari { get; set; } = null!;
        public Rep shb_dwarf { get; set; } = null!;
        public Rep ew_arkasodara { get; set; } = null!;
        public Rep ew_omicron { get; set; } = null!;
        public Rep ew_loporrit { get; set; } = null!;
    }

    public class Rep
    {
        public byte Rank { get; set; } = 0;
        public ushort Value { get; set; } = 0;
    }
}
