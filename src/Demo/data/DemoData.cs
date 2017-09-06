using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace DemoApi.data
{
    public class DemoData : ILoadDemoData<HockeyStat>
    {
        public IQueryable<HockeyStat> Stats { get; set; }

        public DemoData()
        {
            var stats = JsonConvert.DeserializeObject<List<HockeyStat>>(File.ReadAllText("data/NHL2016.json"));
            Stats = stats.AsQueryable();
        }
    }
}