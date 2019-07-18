using ElasticSearchUrbiwise.Models;
using MySql.Data.MySqlClient;
using Nest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchUrbiwise.App.Controllers
{
    class LinkController
    {
        public List<String>[] All()
        {
            int port = 3306;
            string host = "urbiwise.c99cwbn4v1fu.eu-west-3.rds.amazonaws.com";
            string username = "urbiwise_db";
            string password = "8cKPDKkYZBqEhv6W";
            string database = "uda";
            int completed = 0;

            List<string>[] data = new List<string>[1];
            data[0] = new List<string>();

            string connectionString = "datasource=urbiwise.c99cwbn4v1fu.eu-west-3.rds.amazonaws.com;port=3306;username=urbiwise_db;password=8cKPDKkYZBqEhv6W;database=uda;SslMode=none;";

            MySqlConnection connection = new MySqlConnection(connectionString);

            connection.Open();

            try
            {
                if (connection.State == ConnectionState.Open && connection.State.ToString() == "Open")
                {
                    MySqlCommand query = new MySqlCommand("SELECT * FROM links_dicofre;", connection);
                    MySqlDataReader reader = query.ExecuteReader();

                    Console.Write(reader.HasRows);

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ++completed;
                            data[0].Add(reader["dicofre"].ToString());
                            Console.WriteLine("LOAD DATA COMPLETED: " + completed);
                        }
                    }
                }
                else
                {
                    throw new Exception("Não foi possivel buscar os dados na base de dados!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return data;
        }
    }
}
