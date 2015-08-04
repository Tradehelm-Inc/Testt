using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodingExercise
{
    public class Contact : WordBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("outletId")]
        public int OutletId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("profile")]
        public string Profile
        {
            get
            {
                return _parsedString;
            }
            set
            {
                _parsedString = value;
            }
        }
        

        public Contact() 
        {
        }

    }
}
