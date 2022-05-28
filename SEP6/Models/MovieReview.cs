using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP6.Models {
    public class MovieReview {

        public int _id { get; set; }

        public string timestamp { get; set; }

        public int userID { get; set; }

        public string contents { get; set; }
    }
}
