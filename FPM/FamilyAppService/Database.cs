using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace Fbm.FamilyAppService
{
    internal class Database
    {
        private const string Provider = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
        private readonly string _address;
        internal Database(string address)
        {
            _address = address;
        }
        public DataTable GetTable(string tableName)
        {
            DataTable table = new DataTable();
            using (OleDbConnection conn = CreateConnection(_address))
            {
                using (OleDbCommand command = CreateCommand(conn, tableName))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }

        private OleDbConnection CreateConnection(string address)
        { 
            string connectionString = string.Format("{0}{1}", Provider, address);
            return new OleDbConnection(connectionString);
        }
        private OleDbCommand CreateCommand(OleDbConnection connection, string tableName)
        {
            string selecte = "SELECT * FROM " + tableName;
            return new OleDbCommand(selecte, connection);
        }
    }
}
