using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bogus;

namespace DemoApi
{
    public class FilterResponse
    {
        public List<Person> People { get; set; }
        public string FilterText { get; set; }
        public string LinqExpression { get; set; }
    }
}