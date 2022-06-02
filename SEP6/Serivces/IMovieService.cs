using SEP6.Models;

namespace SEP6.Serivces
{
    public interface IMovieService
    {
        Task<MovieData> GetMovie();
        void SetMovie(int id);
        Task<List<MovieData>> GetManyMovies(string movie);
        Task<List<MovieData>> GetPopularMovies();
        Task<List<MovieData>> GetTrendingMovies();
        Task<List<ActorData>> GetActorsAsync();
    }
}
