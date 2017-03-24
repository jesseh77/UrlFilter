using System;
using System.Linq;
using System.Linq.Expressions;
using Nancy;

namespace DemoApi
{
    public class FilterData : NancyModule
    {
        private readonly IQueryable<DemoPerson> _demoData;
        private const int DefaultQuantity = 100;
        public FilterData()
        {
            _demoData = DemoData.GetDemoData(DefaultQuantity);
            Get["/filterData"] = x => FilterTheData();
            Get["/"] = x => Response.AsFile("Content/index.html", "text/html");
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
            var filter = Request.Query["$filter"];
            return filter ?? string.Empty;
        }

        private Expression<Func<DemoPerson, bool>> GetFilterExpression(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return null;
            var expression = UrlFilter.WhereExpression.Build.FromString<DemoPerson>(filter);
            return expression;
        }        
    }
}