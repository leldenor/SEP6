using Microsoft.AspNetCore.Mvc;
using SEP6.Models;
using System.Data.SqlClient;
using System.Text.Json;

namespace SEP6.Serivces {
    public class AccountService : ControllerBase, IAccountService
    {

        private readonly SqlConnectionStringBuilder _connectionString;
        private List<Account> accounts;
        public Account user { get; set; }

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(string username, string password) {

            try {
                using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                    connection.OpenWithRetry();
                    using (var postCommand = connection.CreateCommand()) {
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
                return NotFound(); //failed, return something
            }
            catch (Exception ex) {
                return BadRequest(); // cant connect
            }
        }

        [HttpGet]
        public async Task<Account> GetUser(string username) {

            var accountToGet = new Account();
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
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
                        }
                    }
                    return accountToGet;
                }
            }
        }
        [HttpGet]
        public async Task<int> GetRating(int movieID) {
            var ratingFinal = new int();
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var lookupCommand = connection.CreateCommand()) {
                    lookupCommand.CommandText = @"SELECT RATING, VOTES FROM MOVIE WHERE ID = @movieID";
                    var idLookup = lookupCommand.CreateParameter();
                    idLookup.ParameterName = "@movieID";
                    idLookup.DbType = System.Data.DbType.Int32;
                    idLookup.Value = movieID;
                    lookupCommand.Parameters.Add(idLookup);
                    using (var reader = await lookupCommand.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            var totalScore = int.Parse(reader.GetString(0));
                            var totalVotes = int.Parse(reader.GetString(1));
                            ratingFinal = (totalScore / totalVotes);
                        }
                    }
                    return ratingFinal;
                }
            }
        }
        [HttpGet]
        public async Task<List<MovieReview>> GetReviews(int movieID) {
            var reviewList = new List<MovieReview>();
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var lookupCommand = connection.CreateCommand()) {
                    lookupCommand.CommandText = @"SELECT * FROM REVIEW AS F, MOVIE AS M, MOVIE_REVIEW AS MR WHERE F.ID = MR.REVIEW_ID AND M.ID = MR.MOVIE_ID AND M.ID = @movieID";
                    var idLookup = lookupCommand.CreateParameter();
                    idLookup.ParameterName = "@movieID";
                    idLookup.DbType = System.Data.DbType.Int32;
                    idLookup.Value = movieID;
                    lookupCommand.Parameters.Add(idLookup);
                    using (var reader = await lookupCommand.ExecuteXmlReaderAsync())
                    {
                        while (await reader.ReadAsync()) {
                            reader.MoveToContent();
                            var data = reader.ReadContentAsString();
                            reviewList = JsonSerializer.Deserialize<List<MovieReview>>(data);
                        }
                    }
                    return reviewList;
                }
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostReview(int movieID, string review)
        {
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var postCommand = connection.CreateCommand()) {
                    postCommand.CommandText = @"INSERT INTO MOVIEREVIEWLIST (MOVIEID, USERNAME, TIMESTAMP, CONTENTS) VALUES (@movieID, @username, @date, @review)";
                    var movieIDToDB = postCommand.CreateParameter();
                    movieIDToDB.ParameterName = "@movieID";
                    movieIDToDB.DbType = System.Data.DbType.Int32;
                    movieIDToDB.Value = movieID;
                    postCommand.Parameters.Add(movieIDToDB);
                    var reviewToDB = postCommand.CreateParameter();
                    reviewToDB.ParameterName = "@review";
                    reviewToDB.DbType = System.Data.DbType.String;
                    reviewToDB.Value = review;
                    postCommand.Parameters.Add(reviewToDB);
                    var userIDToDB = postCommand.CreateParameter();
                    userIDToDB.ParameterName = "@username";
                    userIDToDB.DbType = System.Data.DbType.Int32;
                    userIDToDB.Value = user.Username;
                    postCommand.Parameters.Add(userIDToDB);
                    var dateToDB = postCommand.CreateParameter();
                    dateToDB.ParameterName = "@date";
                    dateToDB.DbType = System.Data.DbType.DateTime;
                    dateToDB.Value = DateTime.Now;
                    postCommand.Parameters.Add(dateToDB);
                    await postCommand.ExecuteNonQueryAsync();
                }
            }
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddRating(int movieId, decimal rating) {
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var postCommand = connection.CreateCommand()) {
                    postCommand.CommandText = @"UPDATE MOVIE SET RATING = RATING + @rating, SET VOTES = VOTES + 1 WHERE MOVIE ID = @movieId ";
                    var ratingToDB = postCommand.CreateParameter();
                    ratingToDB.ParameterName = "@movieId";
                    ratingToDB.DbType = System.Data.DbType.Int16;
                    ratingToDB.Value = rating;
                    await postCommand.ExecuteNonQueryAsync();
                }
                
            }
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddFavouriteMovie(int movieId) {
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var postCommand = connection.CreateCommand()) {
                    postCommand.CommandText = @"INSERT INTO USERMOVIE MovieID VALUE @movieid WHERE USERID = @userId ";
                    var movieToDB = postCommand.CreateParameter();
                    movieToDB.ParameterName = "@movieid";
                    movieToDB.DbType = System.Data.DbType.Int32;
                    movieToDB.Value = movieId;
                    postCommand.Parameters.Add(movieToDB);
                    var idToDB = postCommand.CreateParameter();
                    idToDB.ParameterName = "@userId";
                    idToDB.DbType = System.Data.DbType.Int32;
                    idToDB.Value = user._id;
                    postCommand.Parameters.Add(idToDB);
                    await postCommand.ExecuteNonQueryAsync();
                }
            }
            return Ok();
        }
        [HttpGet]
        public async Task<List<MovieData>> GetFavouriteMovies() {
            var movieList = new List<MovieData>();
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var lookupCommand = connection.CreateCommand()) {
                    lookupCommand.CommandText = @"SELECT * FROM MOVIE as M, USER an U, USERMOVIE as UM WHERE M.ID = UM.MOVIEID AND U.ID = UM.USERID AND U.USERNAME = @username";
                    var userIdToDB = lookupCommand.CreateParameter();
                    userIdToDB.ParameterName = "@userId";
                    userIdToDB.DbType = System.Data.DbType.Int32;
                    userIdToDB.Value = user._id;
                    lookupCommand.Parameters.Add(userIdToDB);
                    using (var reader = await lookupCommand.ExecuteXmlReaderAsync()) { //idk if we can use this or not
                        while (await reader.ReadAsync()) {
                            reader.MoveToContent();
                            var data = reader.ReadContentAsString();
                            movieList = JsonSerializer.Deserialize<List<MovieData>>(data);
                        }
                    }
                    return movieList;
                }
            }
        }

        public Account CheckLogin(List<Account> accs, string username, string password) {
            Account? accToReturn = null;

            foreach (Account account in accs) {
                if (account.Username == username && account.Password == password) {
                    accToReturn = account;
                }
                else {

                }
            }
            return accToReturn;
        }
    }
}


