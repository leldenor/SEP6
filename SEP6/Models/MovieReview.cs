using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP6.Models {
    public class MovieReview {

        public int _id { get; set; }

        public DateTime timestamp { get; set; }
        public string username { get; set; }

        public string contents { get; set; }
    }
}
