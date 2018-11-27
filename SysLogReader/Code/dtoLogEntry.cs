using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysLogReader.Code
{
    public class dtoLogEntry
    {
        public DateTime? TS { get; set; }
        public char MsgType { get; set; }
        public int Number { get; set; }
        public string Desc { get; set; }
        public string Source { get; set; }
        public DateTime? Processed { get; set; }
        public string Client { get; set; }
        public string Username { get; set; }
        public string ErrorData { get; set; }
        public string SessionData { get; set; }
        public Guid ID { get; set; }
    }
}