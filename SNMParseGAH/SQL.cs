using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Canteen
{
    public class SQL
    {
        private SqlConnection SqlConnection { get; set; }
        private SqlCommand SqlCommand { get; set; }

        public SqlTransaction GetSqlTransaction()
        {
            return SqlCommand.Transaction;
        }
        public SQL()
        {
            SqlConnection = new SqlConnection(GetConnectionString());
            SqlCommand = new SqlCommand("", SqlConnection);
            if (SqlConnection.State == System.Data.ConnectionState.Closed)
            {
                SqlConnection.Open();
            }
        }
        private string GetConnectionString()
        {
            string connString = @"Data Source = swms22up; Initial Catalog = inventory; Integrated Security = True; User ID = gah\shnyakin; Password=heS00yam";
            //var connString = Properties.Settings.Default.canteenTestConnectionString;
            return connString;
        }
        public void BeginTransaction(SqlTransaction transaction = null)
        {
            if(transaction == null)
            {
                SqlCommand.Transaction = SqlConnection.BeginTransaction();
            }
            else
            {
                SqlCommand.Transaction = transaction;
            }
            
        }
        public void Commit()
        {
            SqlCommand.Transaction.Commit();
        }
        public void RollBack()
        {
            SqlCommand.Transaction.Rollback();
        }

        public SqlDataReader ExecuteQuery(string cmd = "")
        {
            if (cmd != "")
            {
                SqlCommand.CommandText = cmd;
            }
            else
            {
                SqlCommand = new SqlCommand(cmd, SqlConnection);
            }

            return SqlCommand.ExecuteReader();
        }
        public int ExecuteNonQuery(string cmd = "")
        {
            if (cmd != "")
            {
                SqlCommand.CommandText = cmd;
            }
            return SqlCommand.ExecuteNonQuery();
        }
        public void SetSqlCommand(string cmd)
        {
            SqlCommand.CommandText = cmd;
        }
        public SqlDataAdapter QueryForDataAdapter(string selectCMD = "", string deleteCMD = "", string updateCMD = "")
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter
            {
                SelectCommand = new SqlCommand(selectCMD, SqlConnection),
                DeleteCommand = new SqlCommand(deleteCMD, SqlConnection),
                UpdateCommand = new SqlCommand(updateCMD, SqlConnection)
            };
            return dataAdapter;
        }
        public void SetSqlParameters(List<SqlParameter> sqlParameterCollection)
        {
            SqlCommand.Parameters.Clear();
            SqlCommand.Parameters.AddRange(sqlParameterCollection.ToArray());
            for (int i = 0; i < SqlCommand.Parameters.Count; i++)
            {
                if (SqlCommand.Parameters[i].SqlDbType == System.Data.SqlDbType.DateTime)
                {
                    SqlCommand.Parameters[i].SqlDbType = System.Data.SqlDbType.DateTime;
                }
            }
        }
        public void Close()
        {
            if (SqlConnection.State != System.Data.ConnectionState.Closed)
            {
                SqlConnection.Close();
            }
        }
    }
}
