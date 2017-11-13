﻿using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Teradata.Client.Provider;
using Wexflow.Core;

namespace Wexflow.Tasks.SqlToCsv
{
    public enum Type
    {
        SqlServer,
        Access,
        Oracle,
        MySql,
        Sqlite,
        PostGreSql,
        Teradata
    }

    public class SqlToCsv : Task
    {
        public Type DbType { get; set; }
        public string ConnectionString { get; set; }
        public string SqlScript { get; set; }
        public string Separator { get; set; }

        public SqlToCsv(XElement xe, Workflow wf)
            : base(xe, wf)
        {
            DbType = (Type)Enum.Parse(typeof(Type), GetSetting("type"), true);
            ConnectionString = GetSetting("connectionString");
            SqlScript = GetSetting("sql", string.Empty);
            Separator = GetSetting("separator", ";");
        }

        public override TaskStatus Run()
        {
            Info("Executing SQL scripts...");

            bool success = true;
            bool atLeastOneSucceed = false;

            // Execute SqlScript if necessary
            try
            {
                if (!string.IsNullOrEmpty(SqlScript))
                {
                    ExecuteSql(SqlScript);
                    Info("The script has been executed through the sql option of the task.");
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while executing sql script. Error: {0}", e.Message);
                success = false;
            }

            // Execute SQL files scripts
            foreach (FileInf file in SelectFiles())
            {
                try
                {
                    var sql = File.ReadAllText(file.Path);
                    ExecuteSql(sql);
                    InfoFormat("The script {0} has been executed.", file.Path);

                    if (!atLeastOneSucceed) atLeastOneSucceed = true;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    ErrorFormat("An error occured while executing sql script {0}. Error: {1}", file.Path, e.Message);
                    success = false;
                }
            }

            var status = Status.Success;

            if (!success && atLeastOneSucceed)
            {
                status = Status.Warning;
            }
            else if (!success)
            {
                status = Status.Error;
            }

            Info("Task finished.");
            return new TaskStatus(status, false);
        }

        void ExecuteSql(string sql)
        {
            switch (DbType)
            {
                case Type.SqlServer:
                    using (var conn = new SqlConnection(ConnectionString))
                    using (var comm = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        var reader = comm.ExecuteReader();
                        if (reader.HasRows)
                        {
                            var columns = new List<string>();
                            StringBuilder builder = new StringBuilder();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                                builder.Append(reader.GetName(i)).Append(Separator);
                            }

                            builder.Append("\r\n");
                            string destPath = Path.Combine(Workflow.WorkflowTempFolder,
                                                           string.Format("SqlServer_{0:yyyy-MM-dd-HH-mm-ss-fff}.csv",
                                                           DateTime.Now));

                            while (reader.Read())
                            {
                                foreach (var column in columns)
                                {
                                    builder.Append(reader[column]).Append(Separator);
                                }
                                builder.Append("\r\n");
                            }

                            File.WriteAllText(destPath, builder.ToString());
                            Files.Add(new FileInf(destPath, Id));
                            InfoFormat("CSV file generated: {0}", destPath);
                        }
                    }
                    break;
                case Type.Access:
                    using (var conn = new OleDbConnection(ConnectionString))
                    using (var comm = new OleDbCommand(sql, conn))
                    {
                        conn.Open();
                        var reader = comm.ExecuteReader();

                        if (reader.HasRows)
                        {
                            var columns = new List<string>();
                            StringBuilder builder = new StringBuilder();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                                builder.Append(reader.GetName(i)).Append(Separator);
                            }

                            builder.Append("\r\n");
                            string destPath = Path.Combine(Workflow.WorkflowTempFolder,
                                                           string.Format("Access_{0:yyyy-MM-dd-HH-mm-ss-fff}.csv",
                                                           DateTime.Now));

                            while (reader.Read())
                            {
                                foreach (var column in columns)
                                {
                                    builder.Append(reader[column]).Append(Separator);
                                }
                                builder.Append("\r\n");
                            }

                            File.WriteAllText(destPath, builder.ToString());
                            Files.Add(new FileInf(destPath, Id));
                            InfoFormat("CSV file generated: {0}", destPath);
                        }
                    }
                    break;
                case Type.Oracle:
                    using (var conn = new OracleConnection(ConnectionString))
                    using (var comm = new OracleCommand(sql, conn))
                    {
                        conn.Open();
                        var reader = comm.ExecuteReader();

                        if (reader.HasRows)
                        {
                            var columns = new List<string>();
                            StringBuilder builder = new StringBuilder();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                                builder.Append(reader.GetName(i)).Append(Separator);
                            }

                            builder.Append("\r\n");
                            string destPath = Path.Combine(Workflow.WorkflowTempFolder,
                                                           string.Format("Oracle_{0:yyyy-MM-dd-HH-mm-ss-fff}.csv",
                                                           DateTime.Now));

                            while (reader.Read())
                            {
                                foreach (var column in columns)
                                {
                                    builder.Append(reader[column]).Append(Separator);
                                }
                                builder.Append("\r\n");
                            }

                            File.WriteAllText(destPath, builder.ToString());
                            Files.Add(new FileInf(destPath, Id));
                            InfoFormat("CSV file generated: {0}", destPath);
                        }
                    }
                    break;
                case Type.MySql:
                    using (var conn = new MySqlConnection(ConnectionString))
                    using (var comm = new MySqlCommand(sql, conn))
                    {
                        conn.Open();
                        var reader = comm.ExecuteReader();

                        if (reader.HasRows)
                        {
                            var columns = new List<string>();
                            StringBuilder builder = new StringBuilder();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                                builder.Append(reader.GetName(i)).Append(Separator);
                            }

                            builder.Append("\r\n");
                            string destPath = Path.Combine(Workflow.WorkflowTempFolder,
                                                           string.Format("MySql_{0:yyyy-MM-dd-HH-mm-ss-fff}.csv",
                                                           DateTime.Now));

                            while (reader.Read())
                            {
                                foreach (var column in columns)
                                {
                                    builder.Append(reader[column]).Append(Separator);
                                }
                                builder.Append("\r\n");
                            }

                            File.WriteAllText(destPath, builder.ToString());
                            Files.Add(new FileInf(destPath, Id));
                            InfoFormat("CSV file generated: {0}", destPath);
                        }
                    }
                    break;
                case Type.Sqlite:
                    using (var conn = new SQLiteConnection(ConnectionString))
                    using (var comm = new SQLiteCommand(sql, conn))
                    {
                        conn.Open();
                        var reader = comm.ExecuteReader();

                        if (reader.HasRows)
                        {
                            var columns = new List<string>();
                            StringBuilder builder = new StringBuilder();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                                builder.Append(reader.GetName(i)).Append(Separator);
                            }

                            builder.Append("\r\n");
                            string destPath = Path.Combine(Workflow.WorkflowTempFolder,
                                                           string.Format("Sqlite_{0:yyyy-MM-dd-HH-mm-ss-fff}.csv",
                                                           DateTime.Now));

                            while (reader.Read())
                            {
                                foreach (var column in columns)
                                {
                                    builder.Append(reader[column]).Append(Separator);
                                }
                                builder.Append("\r\n");
                            }

                            File.WriteAllText(destPath, builder.ToString());
                            Files.Add(new FileInf(destPath, Id));
                            InfoFormat("CSV file generated: {0}", destPath);
                        }
                    }
                    break;
                case Type.PostGreSql:
                    using (var conn = new NpgsqlConnection(ConnectionString))
                    using (var comm = new NpgsqlCommand(sql, conn))
                    {
                        conn.Open();
                        var reader = comm.ExecuteReader();

                        if (reader.HasRows)
                        {
                            var columns = new List<string>();
                            StringBuilder builder = new StringBuilder();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                                builder.Append(reader.GetName(i)).Append(Separator);
                            }

                            builder.Append("\r\n");
                            string destPath = Path.Combine(Workflow.WorkflowTempFolder,
                                                           string.Format("PostGreSql_{0:yyyy-MM-dd-HH-mm-ss-fff}.csv",
                                                           DateTime.Now));

                            while (reader.Read())
                            {
                                foreach (var column in columns)
                                {
                                    builder.Append(reader[column]).Append(Separator);
                                }
                                builder.Append("\r\n");
                            }

                            File.WriteAllText(destPath, builder.ToString());
                            Files.Add(new FileInf(destPath, Id));
                            InfoFormat("CSV file generated: {0}", destPath);
                        }
                    }
                    break;
                case Type.Teradata:
                    using (var conn = new TdConnection(ConnectionString))
                    using (var comm = new TdCommand(sql, conn))
                    {
                        conn.Open();
                        var reader = comm.ExecuteReader();

                        if (reader.HasRows)
                        {
                            var columns = new List<string>();
                            StringBuilder builder = new StringBuilder();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                                builder.Append(reader.GetName(i)).Append(Separator);
                            }

                            builder.Append("\r\n");
                            string destPath = Path.Combine(Workflow.WorkflowTempFolder,
                                                           string.Format("Teradata_{0:yyyy-MM-dd-HH-mm-ss-fff}.csv",
                                                           DateTime.Now));

                            while (reader.Read())
                            {
                                foreach (var column in columns)
                                {
                                    builder.Append(reader[column]).Append(Separator);
                                }
                                builder.Append("\r\n");
                            }

                            File.WriteAllText(destPath, builder.ToString());
                            Files.Add(new FileInf(destPath, Id));
                            InfoFormat("CSV file generated: {0}", destPath);
                        }
                    }
                    break;
            }
        }
    }
}
