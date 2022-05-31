using SEP6.Models;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Trending;
using TMDbLib.Objects.People;

namespace SEP6.Serivces
{
    public class MovieService : IMovieService
    {
        public MovieData CurrentMovie { get; set; }

        private string apiKey = "9bce3ad7d5f0f4ec48ec6f30ceccdfe6";
        public async Task<List<MovieData>> GetManyMovies(string movie)
        {
            TMDbClient client = new TMDbClient(apiKey);
            SearchContainer<SearchMovie> results = client.SearchMovieAsync(movie).Result;
            List<MovieData> movies = new();
            foreach (SearchMovie result in results.Results.Take(10))
            {
                movies.Add(new MovieData
                {
                    Id = result.Id,
                    Title = result.Title,
                    Description = result.Overview,
                    Poster = result.PosterPath
                });

            }
            return movies;
        }

        public async Task<List<MovieData>> GetTrendingMovies()
        {
            TMDbClient client = new TMDbClient(apiKey);
            SearchContainer<SearchMovie> popular = await client.GetTrendingMoviesAsync(TimeWindow.Week);
            List<MovieData> movies = new();
            foreach (SearchMovie result in popular.Results.Take(5))
            {
                movies.Add(new MovieData
                {
                    Id = result.Id,
                    Title = result.Title,
                    Description = result.Overview,
                    Poster = result.PosterPath,
                    ReleaseDate = result.ReleaseDate == null ? "Unknown" : result.ReleaseDate.ToString()
                }); ;

            }
            return movies;
        }

        public async Task<List<MovieData>> GetPopularMovies()
        {
            TMDbClient client = new TMDbClient(apiKey);
            SearchContainer<SearchMovie> popular = await client.GetMoviePopularListAsync();
            List<MovieData> movies = new();
            foreach (SearchMovie result in popular.Results.Take(5))
            {
                movies.Add(new MovieData
                {
                    Id = result.Id,
                    Title = result.Title,
                    Description = result.Overview,
                    Poster = result.PosterPath,
                    ReleaseDate = result.ReleaseDate == null ? "Unknown" : result.ReleaseDate.ToString()
                }); ;

            }
            return movies;
        }

        public MovieData GetMovie()
        {
            
            return CurrentMovie;
        }

        public async Task<bool> SetMovie(int id)
        {
            TMDbClient client = new TMDbClient(apiKey);
            Movie movie = await client.GetMovieAsync(id, MovieMethods.Credits | MovieMethods.Videos);
            MovieData movieData = new MovieData
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Overview,
                Poster = movie.PosterPath,
                ReleaseDate = movie.ReleaseDate == null ? "Unknown" : movie.ReleaseDate.ToString(),
                Genres = movie.Genres.Select(x => x.Name).ToList(),
                Actors = movie.Credits.Cast.Select(x => x.Name).ToList(),
                Director = movie.Credits.Crew.Where(x => x.Job == "Director").Select(x => x.Name).FirstOrDefault(),
                Trailer = movie.Videos.Results.Where(x => x.Type == "Trailer").Select(x => x.Key).FirstOrDefault()
            };
            CurrentMovie = movieData;
            return true;
        }

        public async Task<List<ActorData>> GetActorsAsync()
        {
            TMDbClient client = new TMDbClient(apiKey);
            SearchContainer<PersonResult> person = await client.GetPersonListAsync(PersonListType.Popular);
            List<ActorData> actors = new();
            foreach (PersonResult result in person.Results)
            {
                actors.Add(new ActorData
                {
                    Id = result.Id,
                    Name = result.Name,
                    Profile = result.ProfilePath
                });
            }
            return actors;
        }
    }
}
