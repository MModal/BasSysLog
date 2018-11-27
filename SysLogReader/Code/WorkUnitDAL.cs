using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;
using Spheris.Common;

namespace SysLogReader.Code
{
    public class WorkUnitDAL: BaseDAL
    {
        private OracleConnection OraConx;

        public WorkUnitDAL(string ConnectionName)
        {
            OraConx = new OracleConnection();
            OraConx.ConnectionString = DesEncryption.DecryptConnectionStringPassword(ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
        }

        public List<dtoDay> GetJobCounts()
        {
            DateTime start = DateTime.Parse(DateTime.Today.ToString("M/1/yyyy"));
            return GetJobCounts(start, start.AddMonths(1));
        }

        public List<dtoDay> GetJobCounts(DateTime start, DateTime end)
        {
            List<dtoDay> list = new List<dtoDay>();

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(Queries.GetQuery(Query.GetWorkUnitCount), OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.AddWithValue(":MONTH_START", start);
            da.SelectCommand.Parameters.AddWithValue(":MONTH_END", end);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                var e = new dtoDay();
                e.Day = ParseDT(dr["Day"]);
                e.ExtSys = dr["EXT_SYS"].ToString();
                e.Count = ParseInt( dr["COUNT"] );
                
                list.Add(e);
            }

            dt.Dispose();
            da.Dispose();

            return list;
        }

    }
}