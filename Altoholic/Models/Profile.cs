using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altoholic.Models
{
    public class Profile
    {
        public string Title { get; set; } = string.Empty;
        public bool TitleIsPrefix { get; set; } = false;
        public int Grand_Company { get; set; } = 0;
        public int Grand_Company_Rank { get; set; } = 0;
        public byte Race { get; set; }
        public byte Tribe { get; set; }
        public int Gender { get; set; }
        public int City_State { get; set; }
        public int Nameday_Day { get; set; }
        public int Nameday_Month { get; set; }
        public int Guardian { get; set; }
    }
}
