using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP6_Cloud.Models {
    public class Account {

        public int _id { get; set; }

        [Required, MinLength(3, ErrorMessage = "Please Enter Username with at least 3 characters")]
        [JsonPropertyName("Username")]
        public string Username { get; set; }

        [Required, MinLength(3, ErrorMessage = "Please Enter Password with at least 3 characters")]
        [JsonPropertyName("Password")]

        public string Password { get; set; }

        [JsonPropertyName("pictureURL")] public string PictureURL { get; set; }

        [Required] public string email { get; set; }

        //Favourite movies by ID - add later Note : Critical
        //public List<string> FavouriteMovies { get; set; }

        //Friends by ID - add later
        //public List<string> Friends { get; set; }
    }
}

