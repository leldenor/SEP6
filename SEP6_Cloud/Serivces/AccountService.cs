using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using SEP6_Cloud.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SEP6_Cloud.Serivces {
    public class AccountService {

        private readonly SqlConnectionStringBuilder _connectionString;
        private List<Account> accounts;

        public Account user { get; set; }

        public AccountService(SqlConnectionStringBuilder connectionString) {
            this._connectionString = connectionString;
        }


        [HttpGet]
        public async Task<Account> Login(string username, string password)
        {

            Account acc = null;
            var accountList = new List<Account>();
            using (var connection = new SqlConnection(_connectionString.ConnectionString))
            {
                connection.Open();
                using (var lookupCommand = connection.CreateCommand())
                {
                    lookupCommand.CommandText = @"SELECT * FROM USER";
                    using (var reader = await lookupCommand.ExecuteReaderAsync())
                    {
                        // needs to be changed -> create a list and add to it
                        while (await reader.ReadAsync())
                        {
                            accountList.Add(new Account()
                            {
                                _id = int.Parse(reader.GetString(0)),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                            });

                            acc = CheckLogin(accountList, username, password);
                        }
                        user = acc;
                        return acc;
                    }
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {

            try
            {
                using (var connection = new SqlConnection(_connectionString.ConnectionString))
                {
                    connection.Open();
                    using (var postCommand = connection.CreateCommand())
                    {
                        postCommand.CommandText = @"INSERT INTO USER (username, password) VALUES (@username, @password)";
                        var usernameToDB = postCommand.CreateParameter();
                        usernameToDB.ParameterName = "@username";
                        usernameToDB.DbType = System.Data.DbType.String;
                        usernameToDB.Value = username;
                        postCommand.Parameters.Add(usernameToDB);
                        var passwordToDB = postCommand.CreateParameter();
                        passwordToDB.ParameterName = "@password";
                        passwordToDB.DbType = System.Data.DbType.String;
                        passwordToDB.Value = password;
                        postCommand.Parameters.Add(passwordToDB);
                        await postCommand.ExecuteNonQueryAsync();
                    }
                    user = new Account()
                    {
                        Username = username,
                        Password = password
                    };
                }
                return null; //failed, return something
            }
            catch (Exception ex)
            {
                return null; // cant connect
            }
        }


        [HttpGet]
        public async Task<Account> GetUser(string username) {

            var accountToGet = new Account();
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.Open();
                using (var lookupCommand = connection.CreateCommand()) {
                    lookupCommand.CommandText = @"SELECT * FROM USER WHERE USERNAME = @username";
                    var usernameLookup = lookupCommand.CreateParameter();
                    usernameLookup.ParameterName = "@username";
                    usernameLookup.DbType = System.Data.DbType.String;
                    usernameLookup.Value = username;
                    lookupCommand.Parameters.Add(usernameLookup);
                    using (var reader = await lookupCommand.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            accountToGet._id = int.Parse(reader.GetString(0));
                            accountToGet.Username = reader.GetString(1);
                            accountToGet.Password = reader.GetString(2);
                            accountToGet.PictureURL = reader.GetString(3);
                            accountToGet.email = reader.GetString(4);
                        }
                    }
                    return accountToGet;
                }
            }
        }

        public Account CheckLogin(List<Account> accs, string username, string password) {
            Account? accToReturn = null;

            foreach(Account account in accs) {
                if(account.Username == username && account.Password == password) {
                    accToReturn = account;
                }
                else {
                    
                }
            }
            return accToReturn;
        }
    }
}
    

