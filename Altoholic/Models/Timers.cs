using System;
using System.Collections.Generic;

namespace Altoholic.Models
{
    public class JumboCacpotTicket
    {
        public int? Value { get; set; }
        public DateTime? LastCheck { get; set; }
    }
    public class Timers
    {
        public int? MinicacpotAllowances { get; set; }
        public DateTime? MinicacpotLastCheck { get; set; }
        public List<JumboCacpotTicket> JumboCacpotTickets { get; init; } = [];
        public DateTime? JumboCacpotLastCheck { get; set; }
        public int? FashionReportAllowances { get; set; }
        public int? FashionReportHighestScore { get; set; }
        public DateTime? FashionReportLastCheck { get; set; }
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
