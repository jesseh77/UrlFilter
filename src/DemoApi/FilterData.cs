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
            Get["/filterData"] = x => FilterTheData();
        }

        private FilterResponse FilterTheData()
        {
            var filterText = GetFilterText();
            var expression = GetFilterExpression(filterText);
            if (expression == null)
            {
                var response = new FilterResponse
                {
                    People = _demoData.ToList(),
                    FilterText = "",
                    LinqExpression = ""
                };
                return response;
            }

            var filteredResponse = new FilterResponse
            {
                People = _demoData.Where(expression).ToList(),
                FilterText = filterText,
                LinqExpression = expression.ToString()
            };
            return filteredResponse;
        }

        private string GetFilterText()
        {
            var filter = Request.Query["$filter"] as string;
            return filter ?? string.Empty;
        }

        private Expression<Func<Person, bool>> GetFilterExpression(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return null;
            var expression = UrlFilter.WhereExpression.Build.FromString<Person>(filter);
            return expression;
        }

        private static IQueryable<Person> GetDemoData(int quantity)
        {
            return Enumerable.Range(1, quantity).Select(x => new Person()).AsQueryable();
        }
    }
}