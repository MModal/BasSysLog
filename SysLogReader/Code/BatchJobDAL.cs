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
    public class BatchJobDAL : BaseDAL
    {
        private OracleConnection OraConx;

        public BatchJobDAL(string ConnectionName)
        {
            OraConx = new OracleConnection();
            OraConx.ConnectionString = DesEncryption.DecryptConnectionStringPassword(ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
        }

        public List<dtoJobEntry> GetJobTypes()
        {
            List<dtoJobEntry> list = new List<dtoJobEntry>();

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(Queries.GetQuery(Query.GetBatchJobTypes), OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                var e = new dtoJobEntry();
                e.ExtSys = dr["EXT_SYS"].ToString();
                e.JobType = dr["BATCH_JOB_TYPE"].ToString();
                e.Description = dr["DESCR"].ToString();

                list.Add(e);
            }

            dt.Dispose();
            da.Dispose();

            return list;
        }

        public List<dtoJobEntry> GetLog(string extSys, string jobType)
        {
            List<dtoJobEntry> list = new List<dtoJobEntry>();

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(Queries.GetQuery(Query.GetBatchJobsByType), OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.AddWithValue(":EXT_SYS", extSys);
            da.SelectCommand.Parameters.AddWithValue(":BATCH_JOB_TYPE", jobType);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                var e = new dtoJobEntry();
                e.Day = ParseDT(dr["Day"]);
                e.ExtSys = dr["EXT_SYS"].ToString();
                e.JobType = dr["BATCH_JOB_TYPE"].ToString();
                e.Time = ParseInt( dr["Time"] );
                
                list.Add(e);
            }

            dt.Dispose();
            da.Dispose();

            return list;
        }

        public List<dtoSyncTimes> GetSyncTimes(string batch_job_type)
        {
            List<dtoSyncTimes> list = new List<dtoSyncTimes>();

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(Queries.GetQuery(Query.GetSyncTimes), OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.AddWithValue(":batch_job_type", batch_job_type);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                var e = new dtoSyncTimes();
                e.Day = ParseDT(dr["Date"]);
                e.DaysRun = ParseInt(dr["Days run"]);
                e.JobsPerMinute = ParseInt(dr["Jobs per min"]);
                e.MinutesPerDay = ParseInt(dr["Minutes per day"]);
                e.RunMinutes = ParseInt(dr["Run min"]);

                list.Add(e);
            }

            dt.Dispose();
            da.Dispose();

            return list;
        }

        public List<dtoSyncTimes> GetSyncTimes2(string batch_job_type, string ext_sys, int days)
        {
            List<dtoSyncTimes> list = new List<dtoSyncTimes>();

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(Queries.GetQuery(Query.GetSyncTimes2), OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.AddWithValue(":batch_job_type", batch_job_type);
            da.SelectCommand.Parameters.AddWithValue(":ext_sys", ext_sys);
            da.SelectCommand.Parameters.AddWithValue(":days", days);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                var e = new dtoSyncTimes();
                e.Day = ParseDT(dr["Date"]);
                e.DaysRun = ParseInt(dr["Days run"]);
                //e.JobsPerMinute = ParseInt(dr["Jobs per min"]);
                //e.MinutesPerDay = ParseInt(dr["Minutes per day"]);
                e.RunMinutes = ParseInt(dr["Run min"]);

                list.Add(e);
            }

            dt.Dispose();
            da.Dispose();

            return list;
        }
    }
}