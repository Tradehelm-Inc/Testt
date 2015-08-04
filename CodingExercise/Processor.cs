using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace CodingExercise
{
    public class Processor
    {
        private List<string> _arguments = new List<string>();
        private List<Contact> _contactList;
        private List<Outlet> _outletList;
        private Dictionary<int, Contact> _contactHash;
        private Dictionary<int, Outlet> _outletHash;
        private List<Outlet> _returnOutlets = new List<Outlet>();
        private List<Contact> _returnContacts = new List<Contact>();
        private Dictionary<string, List<Outlet>> _outletNames = new Dictionary<string, List<Outlet>>();
        private Dictionary<string, List<Contact>> _contactNames = new Dictionary<string, List<Contact>>();
        private Dictionary<string, List<Contact>> _contactTitles = new Dictionary<string, List<Contact>>();

        public void Process(string[] args)
        {
            ProcessArgumentList(args);
            ReadData();
            ProcessObjects();
            PrepareOutput();
            WriteOutput();
        }


        #region Output

        private void WriteOutput()
        {
            StringBuilder sb = new System.Text.StringBuilder();
            Dictionary<int, int> Encountered = new Dictionary<int, int>();

            //5) The count of both outlets and contacts should be returned
            Console.WriteLine(_contactList.Count.ToString() + " Total contacts");
            Console.WriteLine(_outletList.Count.ToString() + " Total outlets");
            Console.WriteLine(_returnContacts.Count.ToString() + " Matching contacts");
            Console.WriteLine(_returnOutlets.Count.ToString() + " Matching outlets\n");

            if (_returnContacts.Count > 0)
            {
                Console.WriteLine("Matching contacts:");
                Console.WriteLine("Id\tFirstName\tLastName\tTitle");
                Encountered.Clear();
                foreach (Contact c in _returnContacts)
                {
                    if (Encountered.ContainsKey(c.Id)) continue;
                    sb.Clear();
                    sb.Append(c.Id);
                    sb.Append("\t");
                    sb.Append(c.FirstName);
                    sb.Append("\t");
                    if (c.FirstName.Length < 8) sb.Append("\t");
                    sb.Append(c.LastName);
                    sb.Append("\t");
                    if (c.LastName.Length < 8) sb.Append("\t");
                    sb.Append(c.Title);
                    Console.WriteLine(sb.ToString());
                    Encountered.Add(c.Id, 0);
                }
            }
            Console.WriteLine();

            if (_returnOutlets.Count > 0)
            {
                Console.WriteLine("Matching outlets:");
                Console.WriteLine("Id\tName");
                Encountered.Clear();
                foreach (Outlet o in _returnOutlets)
                {
                    if (Encountered.ContainsKey(o.Id)) continue;
                    sb.Clear();
                    sb.Append(o.Id);
                    sb.Append("\t");
                    sb.Append(o.Name);
                    sb.Append("\t");
                    Console.WriteLine(sb.ToString());
                    Encountered.Add(o.Id, 0);
                }
            }
            Console.WriteLine();

        
        }

        private void Output<T>(Dictionary<string, List<T>> Dictionary, List<T> Output, string Argument)
        {
            if (Dictionary.ContainsKey(Argument))
            {
                foreach (T o in Dictionary[Argument]) Output.Add(o);
            }
        }

        private void Output<T>(List<T> List, List<T> Output, string Argument) where T : WordBase
        {
            foreach (T o in List)
            {
                if ((o as WordBase).Words.ContainsKey(Argument.ToLower())) Output.Add(o);
            }
        }


        private void PrepareOutput()
        {
            foreach (string a in _arguments)
            {
                Output<Outlet>(_outletList, _returnOutlets, a); //1) MediaOutlets that contain the matching word in the Name should be returned
                Output<Contact>(_contactList, _returnContacts, a); //2) Contacts that contain the matching word in their profile should be returned
                Output<Contact>(_contactNames, _returnContacts, a); //3) Contacts that match on Last Name exactly should be returned
                Output<Contact>(_contactTitles, _returnContacts, a); //4) Contacts that match on Title exactly should be returned
            }
        }

        #endregion Output
        
        #region Object Processing

        private void Process<T>(Dictionary<string, List<T>> Dictionary, T Object, string Key)
        {
            PropertyInfo pi = typeof(T).GetProperty(Key);
            string key = (string)pi.GetValue(Object);

            if (Dictionary.ContainsKey(key))
            {
                Dictionary[key].Add(Object);
            }
            else
            {
                List<T> newlist = new List<T>();
                newlist.Add(Object);
                Dictionary.Add(key, newlist);
            }
        }


        private void ProcessObjects()
        {
            _contactHash = ListToHash<int, Contact>(_contactList, "Id");
            _outletHash = ListToHash<int, Outlet>(_outletList, "Id");

            foreach (Contact c in _contactList)
            {
                // Hook up the contact lists in the outlets
                _outletHash[c.OutletId].Contacts.Add(c);

                //3) Contacts that match on Last Name exactly should be returned
                Process<Contact>(_contactNames, c, "LastName");

                //4) Contacts that match on Title exactly should be returned
                Process<Contact>(_contactTitles, c, "Title");
            }
        }


        private Dictionary<S, T> ListToHash<S, T>(List<T> Source, string KeyProperty)
        {
            Dictionary<S, T> result = new Dictionary<S, T>();

            PropertyInfo id = typeof(T).GetProperty(KeyProperty);
            foreach (T t in Source)
            {
                S key = (S)id.GetValue(t);
                result.Add(key, t);
            }
            return result;
        }

        #endregion Object Processing

        #region File Processing

        private void ReadData()
        {
            try
            {
                string Contacts = ReadJsonFile(@"Contacts.json");
                string Outlets = ReadJsonFile(@"Outlets.json");

                _contactList = ProcessJson<List<Contact>>(Contacts);
                _outletList = ProcessJson<List<Outlet>>(Outlets);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private T ProcessJson<T>(string Source)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(Source);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not parse " + typeof(T).ToString() + " Json: " + ex.Message, ex);
            }

        }

        private string ReadJsonFile(string FileName)
        {
            try
            {
                return File.ReadAllText(FileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read file " + FileName + ". " + ex.Message, ex);
            }
        }

        #endregion File Processing

        #region Parameter Processing

        private void ProcessArgumentList(string[] args)
        {
            if (args.Length == 0)
            {
                OutputUsage();
                Environment.Exit(1);
            }

            foreach (string s in args) _arguments.Add(s);
        }


        private void OutputUsage()
        {
            Console.WriteLine("Usage:\n");
            Console.WriteLine("CodingExercise.exe [One or more parameters]\n");
            Console.WriteLine("Place Contacts.json and Outlets.json in the same directory as the executable.\n");
        }

        #endregion Parameter Processing

    }
}
