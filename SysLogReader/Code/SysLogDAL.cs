using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;
using DotNetOpenAuth.OAuth.Messages;
using Spheris.Common;

namespace SysLogReader.Code
{
    public class SysLogDAL : BaseDAL
    {
        public enum MessageType { ASPNETError, Audit, HTTP404, Upload, Debug, Download };

        public class MsgType
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public static List<MsgType> MsgTypes
        {
            get
            {
                return new List<MsgType>()
                    {
                        new MsgType(){Name="ASPNETError",Value="E"},
                        new MsgType(){Name="Audit",Value="T"},
                        new MsgType(){Name="HTTP404",Value="4"},
                        new MsgType(){Name="Upload",Value="U"},
                        new MsgType(){Name="Debug",Value="B"},
                        new MsgType(){Name="Download",Value="D"},
                    };
            }
        }

        private OracleConnection OraConx;

        public SysLogDAL(string ConnectionName)
        {
            OraConx = new OracleConnection();
            OraConx.ConnectionString = DesEncryption.DecryptConnectionStringPassword(ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
        }

        public List<dtoLogEntry> GetLog(SysLogDAL.MsgType msgType, int recordCount)
        {
            List<dtoLogEntry> list;

            string sql = Queries.GetQuery(Query.GetLog);
            if (msgType != null) sql = Queries.GetQuery(Query.GetLogFilter);

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(sql, OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.AddWithValue(":RECORD_COUNT", recordCount);
            if (msgType != null) da.SelectCommand.Parameters.AddWithValue(":MSG_TYPE", msgType.Value);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            list = ReadRecords(dt);

            dt.Dispose();
            da.Dispose();

            return list;
        }

        private List<dtoLogEntry> ReadRecords(DataTable dt)
        {
            var list = new List<dtoLogEntry>();
            foreach (DataRow dr in dt.Rows)
            {
                var e = new dtoLogEntry();
                e.Client = dr["CLIENT"].ToString();
                e.Desc = dr["DESCRIPTION"].ToString();
                e.MsgType = dr["MSG_TYPE"].ToString().ToCharArray()[0];
                e.Number = ParseInt(dr["NUM"]);
                e.Processed = ParseDT(dr["Processed"]);
                e.Source = dr["SOURCE"].ToString();
                e.TS = ParseDT(dr["TS"]);
                e.Username = dr["USERNAME"].ToString();
                e.ID = new Guid((byte[])dr["ID"]);

                list.Add(e);
            }
            return list;
        }

        public List<dtoLogEntry> GetLogRecord(string id)
        {
            List<dtoLogEntry> list;

            string sql = Queries.GetQuery(Query.GetLogRecord);

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(sql, OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.AddWithValue(":id", id);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ":" + OraConx.ConnectionString);
            }

            list = ReadRecords(dt);

            dt.Dispose();
            da.Dispose();

            return list;
        }

        public List<dtoLogEntry> GetLogByDesc(string description, int recordCount)
        {
            List<dtoLogEntry> list;

            string sql = Queries.GetQuery(Query.GetLogByDesc);

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(sql, OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.AddWithValue(":RECORD_COUNT", recordCount);
            da.SelectCommand.Parameters.AddWithValue(":DESCRIPTION", description);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ":" + OraConx.ConnectionString);
            }

            list = ReadRecords(dt);

            dt.Dispose();
            da.Dispose();

            return list;
        }

        public List<dtoLogEntry> GetLog(string msgType)
        {
            List<dtoLogEntry> list = new List<dtoLogEntry>();

            string sql = Queries.GetQuery(Query.GetLog);
            if (msgType != null) sql = Queries.GetQuery(Query.GetLogFilter);

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(sql, OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            if (msgType != null) da.SelectCommand.Parameters.AddWithValue(":MSG_TYPE", msgType);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                var e = new dtoLogEntry();
                e.Client = dr["CLIENT"].ToString();
                e.Desc = dr["DESCRIPTION"].ToString();
                e.MsgType = dr["MSG_TYPE"].ToString().ToCharArray()[0];
                e.Number = ParseInt(dr["NUM"]);
                e.Processed = ParseDT(dr["Processed"]);
                e.Source = dr["SOURCE"].ToString();
                e.TS = ParseDT(dr["TS"]);
                e.Username = dr["USERNAME"].ToString();
                e.ID = new Guid((byte[])dr["ID"]);

                list.Add(e);
            }

            dt.Dispose();
            da.Dispose();

            return list;
        }

        public dtoLogEntry GetEntry(Guid id)
        {
            dtoLogEntry e = null;

            OracleDataAdapter da = new OracleDataAdapter(new OracleCommand(Queries.GetQuery(Query.GetEntry), OraConx));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.AddWithValue(":ID", id.ToByteArray());
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count>0)
            {
                DataRow dr = dt.Rows[0];
                e = new dtoLogEntry
                {
                    Client = dr["CLIENT"].ToString(),
                    Desc = dr["DESCRIPTION"].ToString(),
                    ErrorData = dr["ERROR_DATA"].ToString(),
                    MsgType = dr["MSG_TYPE"].ToString().ToCharArray()[0],
                    Number = ParseInt(dr["NUM"]),
                    Processed = ParseDT(dr["Processed"]),
                    SessionData = dr["SESSION_DATA"].ToString(),
                    Source = dr["SOURCE"].ToString(),
                    TS = ParseDT(dr["TS"]),
                    Username = dr["USERNAME"].ToString(),
                    ID = new Guid( (byte[])dr["ID"] )
                };
            }

            dt.Dispose();
            da.Dispose();

            return e;
        }

        public void MarkProcessed(Guid id)
        {
            OracleCommand cmd = new OracleCommand(Queries.GetQuery(Query.MarkProcessed), OraConx);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue(":ID", id.ToByteArray());
            
            OraConx.Open();
            cmd.ExecuteNonQuery();
            OraConx.Close();
        }

    }
}