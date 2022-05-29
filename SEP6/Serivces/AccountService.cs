﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using SEP6.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SEP6.Serivces {
    public class AccountService {

        private readonly SqlConnectionStringBuilder _connectionString;
        private List<Account> accounts;

        public AccountService(SqlConnectionStringBuilder connectionString) {
            this._connectionString = connectionString;
        }

        [HttpGet]
        public async Task<Account> Login(string username, string password) {

            Account acc = null;
            var accountList = new List<Account>();
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.Open();
                using (var lookupCommand = connection.CreateCommand()) {
                    lookupCommand.CommandText = @"SELECT * FROM USER";
                    using (var reader = await lookupCommand.ExecuteReaderAsync()) {
                        // needs to be changed -> create a list and add to it
                        while (await reader.ReadAsync()) {
                            accountList.Add(new Account() {
                                _id = reader.GetString(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                PictureURL = reader.GetString(3),
                                email = reader.GetString(4)
                            });

                            acc = CheckLogin(accountList, username, password);
                        }
                        return acc;
                    }
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult?> Register(string username, string password) {

            try {
                using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                    connection.Open();
                    using (var registerCommand = connection.CreateCommand()) {
                        registerCommand.CommandText = @"INSERT INTO USER (username, password) VALUES (@username, @password)";
                        var usernameToDB = registerCommand.CreateParameter();
                        usernameToDB.ParameterName = "@username";
                        usernameToDB.DbType = System.Data.DbType.String;
                        usernameToDB.Value = username;
                        registerCommand.Parameters.Add(usernameToDB);
                        var passwordToDB = registerCommand.CreateParameter();
                        passwordToDB.ParameterName = "@password";
                        passwordToDB.DbType = System.Data.DbType.String;
                        passwordToDB.Value = password;
                        registerCommand.Parameters.Add(passwordToDB);
                        await registerCommand.ExecuteNonQueryAsync();
                    }
                }
                return null; //failed, return something
            }
            catch (Exception ex) {
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
                            accountToGet._id = reader.GetString(0);
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
    
