using SEP6.Models;

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
        [HttpGet]
        public async Task<int> GetRating(int movieID) {
            var ratingFinal = new int();
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var lookupCommand = connection.CreateCommand()) {
                    lookupCommand.CommandText = @"SELECT RATING, VOTES FROM MOVIE WHERE MOVIEID = @movieID";
                    var idLookup = lookupCommand.CreateParameter();
                    idLookup.ParameterName = "@movieID";
                    idLookup.DbType = System.Data.DbType.Int32;
                    idLookup.Value = movieID;
                    lookupCommand.Parameters.Add(idLookup);
                    using (var reader = await lookupCommand.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            var totalScore = reader.GetString(0);
                            var totalVotes = reader.GetString(1);
                            ratingFinal = (totalScore.Parse() / totalVotes.Parse());
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
                    lookupCommand.CommandText = @"SELECT * FROM MOVIEREVIEWLIST WHERE MOVIEID = @movieID";
                    var idLookup = lookupCommand.CreateParameter();
                    idLookup.ParameterName = "@movieID";
                    idLookup.DbType = System.Data.DbType.Int32;
                    idLookup.Value = movieID;
                    lookupCommand.Parameters.Add(idLookup);
                    using (var reader = await lookupCommand.ExecuteXmlReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            string JsonReader = JsonConvert.DeserializeXmlNode(reader);
                            reviewList = JsonSerializer.Deserialize<List<MovieReview>>(json);
                        }
                    }
                    return reviewList;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostReview() {
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var postCommand = connection.CreateCommand()) {
                    postCommand.CommandText = @"INSERT INTO USER (username, password) VALUES (@username, @password)";

                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRating(int movieId, int rating) {
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var postCommand = connection.CreateCommand()) {
                    postCommand.CommandText = @"UPDATE MOVIE SET RATING = RATING + @rating, SET VOTES = VOTES + 1 WHERE MOVIE MOVIEID = @movieId ";
                    var ratingToDB = postCommand.CreateParameter();
                    ratingToDB.ParameterName = "@movieId";
                    ratingToDB.DbType = System.Data.DbType.Int16;
                    ratingToDB.Value = rating;
                    await postCommand.ExecuteNonQueryAsync();
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddFavouriteMovie(int movieId, int userId) {
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var postCommand = connection.CreateCommand()) {
                    postCommand.CommandText = @"INSERT INTO FAVOURITEMOVIELIST MovieID VALUE @movieid WHERE USERID = @userId ";
                    var movieToDB = postCommand.CreateParameter();
                    movieToDB.ParameterName = "@movieid";
                    movieToDB.DbType = System.Data.DbType.Int32;
                    movieToDB.Value = movieId;
                    postCommand.Parameters.Add(movieToDB);
                    var idToDB = postCommand.CreateParameter();
                    idToDB.ParameterName = "@userId";
                    idToDB.DbType = System.Data.DbType.Int32;
                    idToDB.Value = userId;
                    postCommand.Parameter.Add(idToDB);
                    await postCommand.ExecuteNonQueryAsync();
                }
            }
        }
        [HttpGet]
        public async Task<List<int>> GetFavouriteMovies(int userId) {
            var movieList = new List<MovieData>();
            using (var connection = new SqlConnection(_connectionString.ConnectionString)) {
                connection.OpenWithRetry();
                using (var lookupCommand = connection.CreateCommand()) {
                    lookupCommand.CommandText = @"SELECT * FROM FAVOURITEMOVIELIST WHERE USERID = @userId";
                    var userIdToDB = lookupCommand.CreateParameter();
                    userIdToDB.ParameterName = "@userId";
                    userIdToDB.DbType = System.Data.DbType.Int32;
                    userIdToDB.Value = userId;
                    lookupCommand.Parameters,Add(userIdToDB);
                    using (var reader = await lookupCommand.ExecuteXmlReaderAsync()) { //idk if we can use this or not
                        while (await reader.ReadAsync()) {
                            string JsonReader = JsonConvert.DeserializeXmlNode(reader);
                            movieList = JsonSerializer.Deserialize<List<MovieData>>(json);
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


