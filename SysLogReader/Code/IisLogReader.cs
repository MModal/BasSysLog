using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SysLogReader.Code
{
    public class IisLogReader
    {
        private string[] columns;
        private string _filename;
        private System.Data.SqlClient.SqlConnection _cnn;

        public IisLogReader()
        {
            _cnn = new SqlConnection("Data Source=(local);Initial Catalog=Test;Integrated Security=true;");
        }

        public void ReadLog(string file)
        {
            _filename = System.IO.Path.GetFileName(file);
            var lineNum = 0;
            var sr = new System.IO.StreamReader(file);

            _cnn.Open();
            while (sr.Peek() > -1)
            {
                lineNum++;
                var line = sr.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {}
                else if (line.StartsWith("#"))
                {
                    if (line.StartsWith("#Fields"))
                    {
                        line = line.Substring(line.IndexOf(':')+1).Trim();

                        columns = line.Split(' ');
                    }
                }
                else
                {
                    ParseData(line, lineNum);
                }
            }
            sr.Close();
            _cnn.Close();
        }

        private void ParseData(string line, int lineNum)
        {
            var fieldSql = "";
            var valueSql = "";
            var datetime = "";
            var fields = line.Split(' ');

            for(int x=0;x<fields.Length;x++)
            {
                fieldSql += "[" + columns[x] + "],";
                valueSql += "'" + fields[x].Replace("'", "''") + "',";
                if (columns[x] == "date")
                {
                    datetime = fields[x] + datetime;
                }
                else if (columns[x] == "time")
                {
                    datetime = datetime + " " + fields[x];
                }
            }

            fieldSql = fieldSql.TrimEnd(',');
            valueSql = valueSql.TrimEnd(',');

            InsertRecord(fieldSql, valueSql, lineNum, datetime);
        }

        private void InsertRecord(string fieldSql, string valueSql, int lineNum, string datetime)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = _cnn;

                cmd.CommandText = "select Line from IISLog where [File] = @file and [Line] = @line";
                cmd.Parameters.AddWithValue("@file", _filename);
                cmd.Parameters.AddWithValue("@line", lineNum);

                var o = cmd.ExecuteScalar();

                if (o == null || o == DBNull.Value)
                {
                    //record doesn't exist.  Add it
                    cmd.Parameters.Clear();

                    fieldSql = "[File], [Line], [date-time], " + fieldSql;
                    valueSql = "'" + _filename.Replace("'", "''") + "'," + lineNum + ",'" + datetime + "'," + valueSql;

                    cmd.CommandText = "insert into IISLog (" + fieldSql + ") values(" + valueSql + ")";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Close()
        {
            _cnn.Close();
            _cnn.Dispose();
        }
    }
}