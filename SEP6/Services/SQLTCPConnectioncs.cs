using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SEP6.Services {
    public class SQLTCPConnectioncs {

        public static MySqlConnectionStringBuilder NewMysqlTCPConnectionString() {
            var connectionString = new MySqlConnectionStringBuilder() {
                SslMode = MySqlSslMode.Disabled,
                Server = Environment.GetEnvironmentVariable("Instance HOST"), // ip address
                UserID = Environment.GetEnvironmentVariable("DB_USER"),
                Password = Environment.GetEnvironmentVariable("DB_PASS"),
                Database = Environment.GetEnvironmentVariable("DB_NAME"),
                ConnectionProtocol = MySqlConnectionProtocol.UnixSocket
            };
            connectionString.Pooling = true;
            return connectionString;
        }
    }
}
