using System.Collections.Generic;

namespace DemoApi
{
    public class FilterResponse
    {
        public List<DemoPerson> People { get; set; }
        public string FilterText { get; set; }
        public string LinqExpression { get; set; }
    }
}