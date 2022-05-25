using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;

namespace SEP6.TestFunctions {
    public class GetMovie {
 
        public async void getMovie(int movieID, string apiKey) {
            TMDbClient client = new TMDbClient(apiKey);
            Movie movie = await client.GetMovieAsync(movieID);

            Console.WriteLine($"Movie name: {movie.Title}");
        }

        public async void getActor(int actorID, string apiKey) {
            TMDbClient client = new TMDbClient(apiKey);
            Person person = await client.GetPersonAsync(actorID);

            Console.WriteLine($"Person name : {person.Name}");
        }
    }
}
