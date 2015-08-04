using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodingExercise
{
    public class Outlet : WordBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name
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
        
        public List<Contact> Contacts { get; set; }

        public Outlet()
        {
            Contacts = new List<Contact>();
        }
    }
}
