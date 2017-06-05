using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DemoApi.data
{
    public class Loader
    {
        public List<HockeyStat> Stats { get; set; }

        public void LoadData()
        {
            JsonTextReader reader = new JsonTextReader(new StreamReader("Hockey.json"));
            Stats = JsonConvert.DeserializeObject<List<HockeyStat>>(File.ReadAllText("Hockey.json"));

        }
    }
}