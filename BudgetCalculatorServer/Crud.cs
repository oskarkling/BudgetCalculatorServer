using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace BudgetCalculatorServer
{
    public class Crud
    {
        #region Public Methods
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        /// <summary>
        /// Constructor with the connectionstring
        /// </summary>
        public Crud()
        {
            ConnectionString = "workstation id=budgetdb.mssql.somee.com;packet size=4096;user id=budgetadmin;pwd=budget123;data source=budgetdb.mssql.somee.com;persist security info=False;initial catalog=budgetdb";

        }

        public DataTable GetDataTable(string sqlString, params (string, string)[] parameters)
        {
            var dt = new DataTable();
            try
            {
                using (var sqlConnection = new SqlConnection(FinishedConnString()))
                {
                    using (var sqlCmd = new SqlCommand(sqlString, sqlConnection))
                    {
                        InitParameters(parameters, sqlCmd);
                        sqlConnection.Open();
                        try
                        {
                            using (var sqlAdapter = new SqlDataAdapter(sqlCmd))
                            {
                                sqlAdapter.Fill(dt);
                            }
                        }
                        catch (Exception msg)
                        {
                            Console.WriteLine("Error filling sqlAdapter with rows\n " + msg.Message);
                        }
                    }
                }
            }
            catch (Exception msg)
            {
                Console.WriteLine("Error getting datatable\n " + msg.Message);
            }
            return dt;
        }

        public long ExecuteSqlQuery(string sqlText, params (string, string)[] parameters)
        {
            long rowsAffected = 0;
            try
            {
                using (var sqlConnection = new SqlConnection(FinishedConnString()))
                {
                    using (var sqlCmd = new SqlCommand(sqlText, sqlConnection))
                    {
                        InitParameters(parameters, sqlCmd);
                        sqlConnection.Open();
                        rowsAffected = sqlCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception msg)
            {
                Console.WriteLine("Error running sql query\n " + msg);
            }
            return rowsAffected;
        }



        public DataTable GetUserByUsername(string input)
        {
            var sqlText = $@"
SELECT * FROM Accounts
WHERE Username = @Input";
            var dt = GetDataTable(sqlText, ("@Input", input));
            return dt;
        }

        public DataTable GetUserByPassword(string input)
        {
            var sqlText = $@"
SELECT * FROM Accounts
WHERE Password = @Input";
            var dt = GetDataTable(sqlText, ("@Input", input));
            return dt;
        }

        public bool UsernameAndPasswordMatch(string username, string password)
        {
            DataTable dtUsername = GetUserByUsername(username);

            string incomingPassword = "";
            if (dtUsername.Rows.Count > 0)
            {
                foreach (DataRow row in dtUsername.Rows)
                {
                    incomingPassword = row["Password"].ToString();
                }
            }

            if(incomingPassword == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //        public DataTable GetAllWithLetterStart(string input)
        //        {
        //            input = input + "%";
        //            var sqlText = $@"
        //SELECT * FROM Family
        //WHERE FirstName LIKE @Input;";
        //            var dt = GetDataTable(sqlText, ("@Input", input));
        //            return dt;
        //        }

        //        public void DeleteFromTable(int id)
        //        {
        //            var sqlText = $"DELETE FROM Family WHERE id = {@id}";
        //            ExecuteSqlQuery(sqlText, ("@id", id.ToString()));
        //        }

        private bool DoesDatabaseExist(string databaseName)
        {
            bool answer = false;
            var cmdText = $"SELECT COUNT(*) FROM master.dbo.sysdatabases WHERE name = @databaseName";
            using (var sqlConnection = new SqlConnection(FinishedConnString()))
            {
                using (var sqlCmd = new SqlCommand(cmdText, sqlConnection))
                {
                    sqlCmd.Parameters.AddWithValue("@databaseName", databaseName);
                    sqlConnection.Open();
                    if (Convert.ToInt32(sqlCmd.ExecuteScalar()) == 1)
                    {
                        answer = true;
                    }
                    return answer;
                }
            }
        }

        /// <summary>
        /// Initializes Parameters for the sql connection
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="cmd"></param>
        private void InitParameters((string, string)[] parameters, SqlCommand sqlCmd)
        {
            foreach ((string, string) s in parameters)
            {
                sqlCmd.Parameters.AddWithValue(s.Item1, s.Item2);
            }
        }

        /// <summary>
        /// Formats the connectionstring with database name
        /// </summary>
        /// <returns></returns>
        private string FinishedConnString()
        {
            return string.Format(ConnectionString, DatabaseName);
        }

        #endregion Private Methods
    }
}
