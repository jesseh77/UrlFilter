using System.Collections.Generic;

namespace DemoApi
{
    public class FilterResponse<T> where T : class
    {
        public List<T> Values { get; set; }
        public string FilterText { get; set; }
        public string LinqExpression { get; set; }
    }
}