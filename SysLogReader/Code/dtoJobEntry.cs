using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysLogReader.Code
{
    public class dtoJobEntry
    {
        public DateTime? Day { get; set; }
        public string ExtSys { get; set; }
        public string JobType { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
    }
}