using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace CDSWDXListener
{
    /// <summary>
    /// Helper class to access the data from SQL DB.
    /// </summary>
    internal sealed class SqlDataManager
    {
        #region private members
        /// <summary>
        /// Worksource1 database connectionstring
        /// </summary>
        private static string _connectionString = ConfigurationManager.ConnectionStrings["AzureStaging"].ConnectionString;
        #endregion

        #region internal methods
        /// <summary>
        /// Method to execute the given query/sp and return the data from SQL DB.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="IsSP"></param>
        /// <returns></returns>
        internal static DataTable ExecuteQuery(string query, bool IsSP)
        {
            DataTable dataTable = new DataTable("QueryResult");
            dataTable.Locale = CultureInfo.InvariantCulture;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandType = IsSP == true ? CommandType.StoredProcedure : CommandType.Text;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }
        /// <summary>
        /// Method to execute the given query.
        /// </summary>
        /// <param name="query"></param>
        internal static void ExecuteNonQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        internal static string CreateAccount(Guid accountId, string accountName, string accountNumber, string description, string webSiteUrl, string emailAddress, string telephone1, Guid createdById, Guid ownerId, int stateCode = 0, int statusCode = 1)
        {
            Console.WriteLine("Entering SQLDataManager.CreateAccount");

            string rc = "";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("CreateAccount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@AccountId", accountId));
                    command.Parameters.Add(new SqlParameter("@AccountName", accountName));
                    command.Parameters.Add(new SqlParameter("@AccountNumber", accountNumber));
                    command.Parameters.Add(new SqlParameter("@Description", description));
                    command.Parameters.Add(new SqlParameter("@WebsiteUrl", webSiteUrl));
                    command.Parameters.Add(new SqlParameter("@EmailAddress1", emailAddress));
                    command.Parameters.Add(new SqlParameter("@Telephone1", telephone1));
                    command.Parameters.Add(new SqlParameter("@CreatedById", createdById));
                    command.Parameters.Add(new SqlParameter("@OwnerId", ownerId));
                    command.Parameters.Add(new SqlParameter("@StateCode", stateCode));
                    command.Parameters.Add(new SqlParameter("@StatusCode", statusCode));

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        rc = "CreateAccount Command Complete Successfully";
                    }
                    catch (Exception ex)
                    {
                        rc = ex.Message;
                        Console.WriteLine(ex.Message);

                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }

                return rc;

            }
        }

        internal static string UpdateAccount(Guid accountId, string accountName, string accountNumber, string description, string webSiteUrl, string emailAddress, string telephone1, Guid modifiedById, Guid ownerId, int stateCode = 0, int statusCode = 1)
        {
            string rc = "";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateAccount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@AccountId", accountId));
                    command.Parameters.Add(new SqlParameter("@AccountName", accountName));
                    command.Parameters.Add(new SqlParameter("@AccountNumber", accountNumber));
                    command.Parameters.Add(new SqlParameter("@Description", description));
                    command.Parameters.Add(new SqlParameter("@WebsiteUrl", webSiteUrl));
                    command.Parameters.Add(new SqlParameter("@EmailAddress1", emailAddress));
                    command.Parameters.Add(new SqlParameter("@Telephone1", telephone1));
                    command.Parameters.Add(new SqlParameter("@ModifiedById", modifiedById));
                    command.Parameters.Add(new SqlParameter("@OwnerId", ownerId));
                    command.Parameters.Add(new SqlParameter("@StateCode", stateCode));
                    command.Parameters.Add(new SqlParameter("@StatusCode", statusCode));

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        rc = "Command Complete Successfully";
                    }
                    catch (Exception ex)
                    {
                        rc = ex.Message;

                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }

                Console.WriteLine(rc);
                return rc;

            }

        }




        #endregion
    }
}
