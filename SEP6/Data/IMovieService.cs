using SEP6.Models;

namespace SEP6.Data
{
    public interface IMovieService
    {
        Task<List<MovieData>> GetManyMovies(string apiKey, string movie);
        Task<List<MovieData>> GetPopularMovies(string apiKey);
        Task<List<MovieData>> GetTrendingMovies(string apiKey);
    }
}
