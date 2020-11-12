using System;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Net.Mail;
using System.Configuration;

namespace Validate.Lib
{

    /// <summary>
    /// The dbase manager.
    /// </summary>
    public class DbaseManager
    {

        /// <summary>
        /// Gets or Sets the connection.
        /// </summary>
        private SqlConnection Connection { get; set; }

        /// <summary>
        /// Gets or Sets the table.
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// Gets or Sets the is debug.
        /// </summary>
        public bool? IsDebug { get; set; }

        /// <summary>
        /// The .ctor.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DbaseManager(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
        }

        /// <summary>
        /// The record exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result.</returns>
        private bool RecordExists(string key, string value)
        {
            Connection.Open();

            string sql = String.Format("SELECT COUNT(*) FROM {0} WHERE {1} = {2}", Table, key, value);
            SqlCommand cmd = new SqlCommand(sql, Connection);
            Int32 count = (Int32)cmd.ExecuteScalar();

            Connection.Close();

            cmd.Dispose();

            return (count > 0) ? true : false;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <returns>The result.</returns>
        public bool ExecuteQuery(string sql)
        {
            Connection.Open();

            SqlCommand cmd = new SqlCommand(sql, Connection);
            int count = cmd.ExecuteNonQuery();

            Connection.Close();

            cmd.Dispose();

            return (count > 0) ? true : false;
        }


        /// <summary>
        /// Gets the procedure fast with parameters.
        /// </summary>
        /// <returns>The procedure fast with parameters.</returns>
        /// <param name="procedure">Procedure.</param>
        /// <param name="parameters">Parameters.</param>
        public void ExecuteProcedureWithParams(string procedure, Dictionary<string, string> parameters)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(procedure, Connection);
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }

                Connection.Open();

                int status = cmd.ExecuteNonQuery();

                Connection.Close();
            }
            catch (Exception e)
            {
                if (IsDebug.Equals(true))
                {
                    System.Console.WriteLine("Exec Procedure Failure: " + e.Message); //turn on for debug
                }

                Connection.Close();
            }
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="values">The values.</param>
        private void Insert(DataRow row, string[] fields, string[] values)
        {
            string sql = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", Table, String.Join(",", fields), String.Join(",", values));

            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.CommandType = CommandType.Text;

            for (int i = 0; i < fields.Length; i++)
            {
                object value = row[fields[i]].ToString();

                if (value == null || (string)value == "")
                    value = DBNull.Value;

                cmd.Parameters.Add(new SqlParameter(values[i], value));
            }

            Connection.Open();

            // DEBUG: To Show values in query
            string query = DebugSqlCommand(cmd);
            // DEBUG: To Show values in query

            int status = cmd.ExecuteNonQuery();

            Connection.Close();

            cmd.Dispose();
        }


        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="values">The values.</param>
        /// <param name="wheres">The wheres.</param>
        private void Update(DataRow row, string[] fields, string[] values, string[] wheres)
        {
            string[] updates = new string[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                updates[i] = String.Format("{0} = {1}", fields[i], values[i]);
            }

            string sql = String.Format("UPDATE {0} SET {1} WHERE {2}", Table, String.Join(", ", updates), String.Join(" AND ", wheres));

            SqlCommand cmd = new SqlCommand(sql, Connection);
            cmd.CommandType = CommandType.Text;

            for (int i = 0; i < fields.Length; i++)
            {
                object value = row[fields[i]].ToString();

                if (value == null || (string)value == "")
                    value = DBNull.Value;

                cmd.Parameters.Add(new SqlParameter(values[i], value));
            }

            Connection.Open();

            // DEBUG: To Show values in query
            string query = DebugSqlCommand(cmd);
            // DEBUG: To Show values in query

            int status = cmd.ExecuteNonQuery();

            Connection.Close();

            cmd.Dispose();
        }

        /// <summary>
        /// The copy.
        /// </summary>
        /// <param name="downloadTable">The download table.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="values">The values.</param>
        internal void Copy(DataTable downloadTable, string primaryKey, string[] fields, string[] values)
        {
            string primaryKeyVal = null;

            foreach (DataRow row in downloadTable.Rows)
            {
                primaryKeyVal = row[primaryKey].ToString();

                if (!this.RecordExists(primaryKey, row[primaryKey].ToString()))
                {
                    if (IsDebug == true)
                    {
                        System.Console.WriteLine("Inserting Record #" + row[primaryKey]);
                    }

                    try
                    {
                        this.Insert(row, fields, values);
                    }
                    catch (Exception e)
                    {
                        if (this.IsDebug.Equals(true))
                        {
                            System.Console.WriteLine("Copy Failure For Record #" + primaryKeyVal + " " + e.Message); //turn on for debug
                        }

                        Connection.Close();

                        continue;
                    }
                }
                else
                {
                    if (IsDebug == true)
                    {
                        System.Console.WriteLine("Updating Record #" + row[primaryKey]);
                    }

                    string[] wheres = { primaryKey + " = " + row[primaryKey].ToString() };

                    try
                    {
                        this.Update(row, fields, values, wheres);
                    }
                    catch (Exception e)
                    {
                        if (this.IsDebug.Equals(true))
                        {
                            System.Console.WriteLine("Copy Failure For Record #" + primaryKeyVal + " " + e.Message); //turn on for debug
                        }

                        Connection.Close();

                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// The debug sql command.
        /// </summary>
        /// <param name="cmd">The cmd.</param>
        /// <returns>The result.</returns>
        private string DebugSqlCommand(SqlCommand cmd)
        {
            string query = cmd.CommandText;
            foreach (SqlParameter p in cmd.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }

            return query;
        }

        /// <summary>
        /// The download.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="wheres">The wheres.</param>
        /// <param name="isDebug">The debug bit.</param>
        /// <returns>The result.</returns>
        public DataTable Download(string[] fields, string[] wheres)
        {
            DataTable dataTable = new DataTable();

            try
            {
                string sql = String.Format("SELECT {0} FROM {1}", String.Join(",", fields), Table);

                if (wheres.Length > 0)
                {
                    sql = String.Format("SELECT {0} FROM {1} WHERE {2}", String.Join(",", fields), Table, String.Join(" AND ", wheres));
                }

                Connection.Open();

                using (SqlCommand command = new SqlCommand(sql, Connection))
                {
                    using (IDataReader rdr = command.ExecuteReader())
                    {
                        dataTable = GetDataTableFromDataReader(rdr);
                    }

                    command.Dispose();
                }

                Connection.Close();
            }
            catch (Exception e)
            {
                if (IsDebug.Equals(true))
                {
                    System.Console.WriteLine("Download Failure: " + e.Message); //turn on for debug
                }

                Connection.Close();
            }

            return dataTable;
        }

        /// <summary>
        /// The append.
        /// </summary>
        /// <param name="downloadTable">The download table.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="values">The values.</param>
        public void Append(DataTable downloadTable, string[] fields, string[] values)
        {
            foreach (DataRow row in downloadTable.Rows)
            {
                try
                {
                    this.Insert(row, fields, values);
                }
                catch (Exception e)
                {
                    if (this.IsDebug.Equals(true))
                    {
                        System.Console.WriteLine("Append Failure For Record " + e.Message); //turn on for debug
                    }

                    Connection.Close();

                    continue;
                }

            }
        }

        /// <summary>
        /// Gets the data table from data reader.
        /// </summary>
        /// <returns>The data table from data reader.</returns>
        /// <param name="dataReader">Data reader.</param>
        private DataTable GetDataTableFromDataReader(IDataReader dataReader)
        {
            DataTable schemaTable = dataReader.GetSchemaTable();
            schemaTable.Locale = CultureInfo.CurrentCulture;

            DataTable resultTable = new DataTable();
            resultTable.Locale = CultureInfo.CurrentCulture;

            foreach (DataRow dataRow in schemaTable.Rows)
            {
                DataColumn dataColumn = new DataColumn();
                dataColumn.ColumnName = dataRow["ColumnName"].ToString();
                dataColumn.DataType = Type.GetType(dataRow["DataType"].ToString());
                dataColumn.ReadOnly = (bool)dataRow["IsReadOnly"];
                dataColumn.AutoIncrement = (bool)dataRow["IsAutoIncrement"];
                dataColumn.Unique = (bool)dataRow["IsUnique"];

                resultTable.Columns.Add(dataColumn);
                dataColumn.Dispose();
            }
            schemaTable.Dispose();

            while (dataReader.Read())
            {
                DataRow dataRow = resultTable.NewRow();
                for (int i = 0; i < resultTable.Columns.Count; i++)
                {
                    dataRow[i] = dataReader[i];
                }
                resultTable.Rows.Add(dataRow);
            }

            return resultTable;
        }
    }
}
