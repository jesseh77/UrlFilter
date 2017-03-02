using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Bogus;
using Nancy;

namespace DemoApi
{
    public class FilterData : NancyModule
    {
        private readonly IQueryable<Person> _demoData;
        private const int DefaultQuantity = 200;
        public FilterData()
        {
            _demoData = GetDemoData(DefaultQuantity);
            Get["/FilterData"] = x => FilterTheData();
        }

        private List<Person> FilterTheData()
        {
            var expression = GetFilterExpression();
            return _demoData.Where(expression).ToList();
        }

        private Expression<Func<Person, bool>> GetFilterExpression()
        {
            var filter = (string)Request.Query["$filter"];
            if (filter == null) return null;
            var expression = UrlFilter.WhereExpression.Build.FromString<Person>(filter);
            return expression;
        }

        private static IQueryable<Person> GetDemoData(int quantity)
        {
            return Enumerable.Range(1, quantity).Select(x => new Person()).AsQueryable();
        }
    }
}