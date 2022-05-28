using SEP6.Models;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Trending;

namespace SEP6.Data
{
    public class MovieService : IMovieService
    {
        public async Task<List<MovieData>> GetManyMovies(string apiKey, string movie)
        {
            TMDbClient client = new TMDbClient(apiKey);
            SearchContainer<SearchMovie> results = client.SearchMovieAsync(movie).Result;
            List<MovieData> movies = new();
            foreach (SearchMovie result in results.Results.Take(10))
            {
                movies.Add(new MovieData { 
                    Title = result.Title, 
                    Description = result.Overview,
                    Poster = result.PosterPath 
                });

            }
            return movies;
        }

		public async Task<List<MovieData>> GetTrendingMovies(string apiKey)
		{
            TMDbClient client = new TMDbClient(apiKey);
            SearchContainer<SearchMovie> popular = await client.GetTrendingMoviesAsync(TimeWindow.Week);
            List<MovieData> movies = new();
            foreach (SearchMovie result in popular.Results.Take(5))
            {
                movies.Add(new MovieData
                {
                    Title = result.Title,
                    Description = result.Overview,
                    Poster = result.PosterPath,
                    ReleaseDate = result.ReleaseDate == null ? "Unknown" : result.ReleaseDate.ToString()
                }); ;

            }
            return movies;
        }

		public async Task<List<MovieData>> GetPopularMovies(string apiKey)
        {
            TMDbClient client = new TMDbClient(apiKey);
            SearchContainer<SearchMovie> popular = await client.GetMoviePopularListAsync();
            List<MovieData> movies = new();
            foreach (SearchMovie result in popular.Results.Take(5))
            {
                movies.Add(new MovieData
                {
                    Title = result.Title,
                    Description = result.Overview,
                    Poster = result.PosterPath,
                    ReleaseDate = result.ReleaseDate == null ? "Unknown" : result.ReleaseDate.ToString()
                }); ;

            }
            return movies;
        }
    }
}
