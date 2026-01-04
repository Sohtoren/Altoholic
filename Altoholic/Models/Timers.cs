using System;
using System.Collections.Generic;
using System.Text;

namespace Altoholic.Models
{
    public class Timers
    {
        public uint? MinicacpotAllowances { get; init; }
        public DateTime? MinicacpotLastCheck { get; init; }
        public uint? JumboCacpotAllowances { get; init; }
        public DateTime? JumpboCacpotLastCheck { get; init; }
        public int? FashionReportAllowances { get; init; }
        public DateTime? FashionReportLastCheck { get; init; }
        public int? CustomDeliveriesAllowances { get; set; }
        public DateTime? CustomDeliveriesLastCheck { get; set; }
        public int? DomanEnclaveWeeklyAllowances { get; set; }
        public int? DomanEnclaveWeeklyDonation { get; set; }
        public DateTime? DomanEnclaveLastCheck { get; set; }

        public bool MaskedCarnivaleNoviceChallenge { get; set; }
        public bool MaskedCarnivaleModerateChallenge { get; set; }
        public bool MaskedCarnivaleAdvancedChallenge { get; set; }
        public DateTime? MaskedFestivalLastCheck { get; set; }
        public uint? TribeRemainingAllowances { get; set; }
        public DateTime? TribeLastCheck { get; set; }
    }
}
