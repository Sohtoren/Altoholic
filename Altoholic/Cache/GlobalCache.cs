using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altoholic.Cache
{
    public class GlobalCache
    {
        public IconStorage IconStorage { get; set; } = null!;
        public ItemStorage ItemStorage { get; set; } = null!;
        public JobStorage JobStorage{ get; set; } = null!;
    }
}
