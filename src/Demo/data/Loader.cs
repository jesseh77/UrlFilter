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
        public List<Episode> Stats { get; set; }

        public void LoadData()
        {
            Stats = JsonConvert.DeserializeObject<List<Episode>>(File.ReadAllText("SimpsonsEpisodes.json"));
            
        }
    }
}