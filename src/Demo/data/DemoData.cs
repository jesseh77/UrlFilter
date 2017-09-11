using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace DemoApi.data
{
    public class DemoData : ILoadDemoData<HockeyStat>
    {
        private const string demoDataFilePath = "data/NHL2016.json";
        public IQueryable<HockeyStat> Stats { get; set; }

        public DemoData()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, demoDataFilePath);
            var stats = JsonConvert.DeserializeObject<List<HockeyStat>>(File.ReadAllText(path));
            Stats = stats.AsQueryable();
        }
    }
}