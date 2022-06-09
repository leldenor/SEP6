namespace SEP6_Cloud.Models
{
    public class MovieData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Genres { get; set; }
        public string Director { get; set; }
        public List<ActorData> Actors { get; set; }
        public string Language { get; set; }
        public string Poster { get; set; }
        public string ReleaseDate { get; set; }
        public string Trailer { get; set; }
    }
}
