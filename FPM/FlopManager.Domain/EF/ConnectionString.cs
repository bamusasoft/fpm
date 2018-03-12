using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlopManager.Domain.EF
{
    public static class ConnectionString
    {
        public static string ServerName { get; set; }
        public static string FamilyConnection
        {
            get
            {
                string providerName = "System.Data.SqlClient";
                string serverName = ServerName; //".";
                string databaseName = "Family";
                // Initialize the connection string builder for the
                // underlying provider.

                SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
                // Set the properties for the data source.
                sqlBuilder.DataSource = serverName;
                sqlBuilder.InitialCatalog = databaseName;
                //sqlBuilder.IntegratedSecurity = true;
                sqlBuilder.UserID = "abdullah";
                //sqlBuilder.UserID = "bamusa";
                sqlBuilder.Password = "12345678";
                sqlBuilder.MultipleActiveResultSets = true;
                sqlBuilder.ApplicationName = "EntityFramework";


                // Build the SqlConnection connection string.
                string providerString = sqlBuilder.ToString();

                // Initialize the EntityConnectionStringBuilder.
                SqlConnection sqlConnection = new SqlConnection(providerString);

                #region "Entity connection when using EF designer model"

                //EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
                ////Set the provider name.
                //entityBuilder.Provider = providerName;
                //// Set the provider-specific connection string.
                //entityBuilder.ProviderConnectionString = providerString;

                //// Set the Metadata location.
                //entityBuilder.Metadata = @"res://*/DbModel.JsaDbModel.csdl|res://*/DbModel.JsaDbModel.ssdl|res://*/DbModel.JsaDbModel.msl";

                ////Console.WriteLine(entityBuilder.ToString());
                //var s = entityBuilder.ToString();
                //return s;

                #endregion

                var connection = sqlConnection.ConnectionString;
                return connection;

                //string s = @"<add name="JsaDb" connectionString="data source=GHALIB-PC;initial catalog=JsaDb;persist security info=True;user id=abdullah;password=12345678;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />"
            }
        }
    }
}
