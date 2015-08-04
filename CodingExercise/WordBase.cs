using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CodingExercise
{
    public class WordBase
    {
        protected string _parsedString { get; set; }

        private Dictionary<string, string> _words;

        public Dictionary<string, string> Words
        {
            get
            {
                if (_words == null) BuildWords();
                return _words;
            }
        }

        private void BuildWords()
        {
            _words = new Dictionary<string, string>();
            foreach (string word in _parsedString.Split(' '))
            {
                if (!_words.ContainsKey(word.ToLower())) _words.Add(word.ToLower(), "");
            }

        }
    }
}
