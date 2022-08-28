using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace FinTriageGUI
{
   public class userData
   {
        public string id { get; set; }
        public string temperature { get; set; }
   }

    public class Data
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mi { get; set; }
        public string classfication { get; set; }
        public string date { get; set; }
        public string dataUser { get; set; }
        public string temperature { get; set; }
        public int timesIn { get; set; }
        public string message { get; set; }
    }

    public class Root
    {
        public string result { get; set; } 
        public string status { get; set; } 
        public string message { get; set; }
        public Data data { get; set; }
    }

}
