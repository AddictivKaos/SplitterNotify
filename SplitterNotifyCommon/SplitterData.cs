using System;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitterNotifyCommon
{
    public class SplitterData
    {
        private string connectionString;
        private string searchQueryTemplate;

        public SplitterData(NameValueCollection settings)
        {
            connectionString = settings["dbConnectionString"];
            searchQueryTemplate = settings["dbSearchQueryTemplate"];
        }

        public HashSet<string> ExplicitMatch(HashSet<string> addressesToSearch)
        {
            HashSet<string> results = new HashSet<string>();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                foreach (string address in addressesToSearch)
                {
                    string searchQuery = string.Format(searchQueryTemplate, address);
                    using (SqlCommand sqlCommand = new SqlCommand(searchQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string a = (string)reader["address"];
                                results.Add(address);
                                break;
                            }
                        }
                    }
                }
            }
            return results;
        }
    }
}
