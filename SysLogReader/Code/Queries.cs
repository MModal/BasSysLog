using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SysLogReader.Code
{
    public enum Query
    {
        GetLog,
        GetLogFilter,
        GetEntry,
        MarkProcessed,
        GetBatchJobs,
        GetBatchJobsByType,
        GetBatchJobTypes,
        GetWorkUnitCount,
        GetSyncTimes,
        GetSyncTimes2,
        GetLogRecord,
        GetLogByDesc
    }

    public class Queries
    {
        public static string GetQuery(Query query)
        {
            return GetQuery(query.ToString());
        }

        private static string GetQuery(string query)
        {
            string file = System.IO.Path.Combine(System.IO.Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data"), "sql.xml");

            XDocument xdoc = XDocument.Load(file);

            var item = xdoc.Descendants().First(x => x.Attribute("id") != null && x.Attribute("id").Value == query);

            return item.Value;
        }
    }
}