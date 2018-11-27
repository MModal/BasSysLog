using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysLogReader.Code
{
    public class dtoSyncTimes
    {
        public int JobsPerMinute { get; set; }
        public int MinutesPerDay { get; set; }
        public int RunMinutes { get; set; }
        public int DaysRun { get; set; }
        public DateTime? Day { get; set; }
    }
}