using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP6.Models {
    public class Account {

        public string _id { get; set; }

        [Required, MinLength(3, ErrorMessage = "Please Enter Username with at least 3 characters")]
        [JsonPropertyName("Username")]
        public string Username { get; set; }

        [Required, MinLength(1, ErrorMessage = "Please Enter First Name")]
        [JsonPropertyName("Fname")]
        public string Fname { get; set; }

        [Required, MinLength(1, ErrorMessage = "Please Enter Last Name")]
        [JsonPropertyName("Lname")]
        public string Lname { get; set; }

        [JsonPropertyName("pictureURL")] public string PictureURL { get; set; }

        [Required] public string email { get; set; }

        //Favourite movies by ID
        public List<string> FavouriteMovies { get; set; }

        //Friends by ID
        public List<string> Friends { get; set; }
    }
}

