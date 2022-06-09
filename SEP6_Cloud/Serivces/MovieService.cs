using SEP6_Cloud.Models;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Trending;
using TMDbLib.Objects.People;

namespace SEP6_Cloud.Serivces
{
    public class MovieService : IMovieService
    {
        public int CurrentMovieId { get; set; }

        private string apiKey = "9bce3ad7d5f0f4ec48ec6f30ceccdfe6";
        public async Task<List<MovieData>> GetManyMovies(string movie)
        {
            TMDbClient client = new TMDbClient(apiKey);
            SearchContainer<SearchMovie> results = client.SearchMovieAsync(movie).Result;
            List<MovieData> movies = new List<MovieData>();
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
            List<MovieData> movies = new List<MovieData>();
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
            List<MovieData> movies = new List<MovieData>();
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

        public async Task<MovieData> GetMovie()
        {
            TMDbClient client = new TMDbClient(apiKey);
            Movie movie = await client.GetMovieAsync(CurrentMovieId, MovieMethods.Credits | MovieMethods.Videos);
            List<ActorData> actors = new List<ActorData>();
            foreach(Cast person in movie.Credits.Cast)
            {
                actors.Add(new ActorData
                {
                    Id = person.Id,
                    Name = person.Name
                });
            }
            MovieData movieData = new MovieData
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Overview,
                Poster = movie.PosterPath,
                ReleaseDate = movie.ReleaseDate == null ? "Unknown" : movie.ReleaseDate.ToString(),
                Genres = movie.Genres.Select(x => x.Name).ToList(),
                Actors = actors,
                Director = movie.Credits.Crew.Where(x => x.Job == "Director").Select(x => x.Name).FirstOrDefault(),
                Trailer = movie.Videos.Results.Where(x => x.Type == "Trailer").Select(x => x.Key).FirstOrDefault()
            };

            return movieData;
        }

        public void SetMovie(int id)
        {
            CurrentMovieId = id;
        }

        public async Task<List<ActorData>> GetActorsAsync()
        {
            TMDbClient client = new TMDbClient(apiKey);
            SearchContainer<PersonResult> person = await client.GetPersonListAsync(PersonListType.Popular);
            List<ActorData> actors = new List<ActorData>();
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

        public async Task<ActorData> GetActor(int actor)
        {
            TMDbClient client = new TMDbClient(apiKey);
            Person person = await client.GetPersonAsync(actor);
            ActorData actorData = new ActorData
            {
                Id = person.Id,
                Name = person.Name,
                Profile = person.ProfilePath
            };
            return actorData;
        }

        public async Task<List<MovieData>> GetMovieListByActor(int actor)
        {
            TMDbClient client = new TMDbClient(apiKey);
            MovieCredits movie = await client.GetPersonMovieCreditsAsync(actor);
            List<MovieData> movieData = new List<MovieData>();
            foreach (MovieRole item in movie.Cast)
            {
                movieData.Add(new MovieData
                {
                    Id = item.Id,
                    Title = item.Title,
                    Poster = item.PosterPath
                });
            }
            return movieData;
        }
    }
}
