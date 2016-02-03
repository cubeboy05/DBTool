using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUIDatabaseTool
{
    class SimpleDataSource : IDisposable
    {
        public MySqlConnection conn;
        public DataTable dt;
        ///<summary>
        ///Constructor, calls Connect method with params
        ///</summary>
        ///<param name="server"> Server IP or hostname</param>
        ///<param name="database">Database/schema name</param>
        ///<param name="port">Port number</param>
        ///<param name="user">Username</param>
        ///<param name="password">Password</param>
        public SimpleDataSource(string server, string database, int port, string user, string password)
        {
            try
            {
                // TODO: Call the Connect method with the parameters.
                Connect(server, database, port, user, password);
            }
            catch (MySqlException e) { e.StackTrace.ToString(); } 
        } 

        /// <summary>
        /// Intialises MySqlConnection object with parameters provided.
        /// </summary>
        /// <param name="server">Server IP or hostname</param>
        /// <param name="database">Database/schema name</param>
        /// <param name="port">Port number</param>
        /// <param name="user">Username</param>
        /// <param name="password">Password</param>
        public void Connect(string server, string database, int port, string user, string password)
        {
            // TODO: Initialise MySqlConnection object with parameters,
            // open connection with suitable exception handling.
            string connStr = "server=" + server + ";database=" + database + ";port=" + port + ";user=" + user + ";password=" + password + ";";
            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(@"Unable to connect to database. 
                                Please check the following and try again.
                                1. Ensure you have an internet connection.
                                2. Ensure your credentials are entered correctly");
                Console.WriteLine("The error is " + e);
            }
        }

        /// <summary>
        /// Garbage collection method called by Garbage Collector.
        /// SimpleDataSource implements IDisposable, so the GC will
        /// know to call this method when the object is no longer
        /// needed.
        /// </summary>
        public void Dispose()
        {
            if (conn != null)
                conn.Dispose();
        }

        /// <summary>
        /// Updates the datagrid using information from the query string.
        /// </summary>
        /// <param name="queryString"></param>
        public void Query(string queryString)
        {
            MySqlDataAdapter adapter;
            try
            {
                adapter = new MySqlDataAdapter(queryString, conn);
                dt = new DataTable();
                adapter.Fill(dt);
                adapter.Update(dt);
            }
            catch (MySqlException e)
            {
                e.StackTrace.ToString();
            }
        }

        /// <summary>
        /// Preps an SQL statement from the provided string and
        /// executes it.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        public void UpdatePrepare(string statement, string[] keys, string[] values)
        {
            try
            {
                //Declare and Instantiate the MySqlCommand object 
                MySqlCommand cmd = new MySqlCommand();
                //Assign the database connection to it 
                cmd.Connection = conn;
                //Write the command text - NOTE @Name is a placeholder 
                //value that will be overwritten after the statement 
                //is prepared 
                cmd.CommandText = statement;
                //Call the prepare method on the command 
                cmd.Prepare();        
                //Overwrite the @Name parameter with a data value 
                for (int x = 0; x < values.Length; x++)
                {
                    cmd.Parameters.AddWithValue(keys[x], values[x]);
                }
                //Execute the command 
                cmd.ExecuteNonQuery();
            }
            catch (InvalidOperationException e)
            {
                e.StackTrace.ToString();
            }
        }
    }
}
