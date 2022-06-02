using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP6.Models {
    public class Account {

        public int _id { get; set; }

        [Required, MinLength(3, ErrorMessage = "Please Enter Username with at least 3 characters")]
        [JsonPropertyName("Username")]
        public string Username { get; set; }

        [Required, MinLength(3, ErrorMessage = "Please Enter Password with at least 3 characters")]
        [JsonPropertyName("Password")]

        public string Password { get; set; }
    }
}

