using SEP6_Cloud.Models;

namespace SEP6_Cloud.Serivces
{
    public interface IMovieService
    {
        Task<MovieData> GetMovie();
        void SetMovie(int id);
        Task<List<MovieData>> GetManyMovies(string movie);
        Task<List<MovieData>> GetPopularMovies();
        Task<List<MovieData>> GetTrendingMovies();
        Task<List<ActorData>> GetActorsAsync();
        Task<ActorData> GetActor(int actor);
        Task<List<MovieData>> GetMovieListByActor(int actor);
    }
}
