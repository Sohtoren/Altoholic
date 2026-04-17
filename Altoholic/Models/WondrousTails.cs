using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Altoholic.Models
{
    public class WondrousTails
    {
        public bool HasWeeklyBingoJournal { get; set; }
        public uint WeeklyBingoNumSecondChancePoints { get; set; }
        public int WeeklyBingoNumPlacedStickers { get; set; }
        public bool IsWeeklyBingoExpired { get; set; }
        public int[] WeeklyBingoTaskStatus { get; set; } = [16];
        /// <summary>
        /// Will be the last book reset date if HasWeeklyBingoJournal is false
        /// </summary>
        public DateTime WeeklyBingoExpireUnixTimestamp { get; set; }
        public DateTime LastCheck { get; set; }
    }
}
