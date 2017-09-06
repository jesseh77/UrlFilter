using System;
using System.Linq;
using System.Linq.Expressions;
using Nancy;
using DemoApi.data;

namespace DemoApi
{
    public class FilterData : NancyModule
    {
        private const int DefaultQuantity = 100;
        private readonly ILoadDemoData<HockeyStat> demoData;

        public FilterData(ILoadDemoData<HockeyStat> demoData)
        {
            this.demoData = demoData;
            Get["/filterData"] = x => FilterTheData();            
        }

        private FilterResponse<HockeyStat> FilterTheData()
        {
            var filterText = GetFilterText();
            var expression = GetFilterExpression(filterText);
            if (expression == null)
            {
                var response = new FilterResponse<HockeyStat>
                {
                    Values = demoData.Stats.ToList(),
                    FilterText = "",
                    LinqExpression = ""
                };
                return response;
            }

            var filteredResponse = new FilterResponse<HockeyStat>
            {
                Values = demoData.Stats.Where(expression).ToList(),
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

        private Expression<Func<HockeyStat, bool>> GetFilterExpression(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return null;
            var expression = UrlFilter.WhereExpression.Build.FromString<HockeyStat>(filter);
            return expression;
        }        
    }
}