using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DemoApi.data
{
    public class Loader
    {
        public List<HockeyStat> Stats { get; set; }

        public void LoadData()
        {
            Stats = JsonConvert.DeserializeObject<List<HockeyStat>>(File.ReadAllText("NHL2016.json"));            
        }
    }
}