using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace UrlFilter.PerfTests
{
    public class ExpressionGenerationTests
    {
        private static List<string> filters = loadFilters();
        private static WhereExpression expressionProcessor = new WhereExpression();

        [Benchmark]
        public void GenerateExpressions()
        {
            foreach (var filter in filters)
            {
                var result = expressionProcessor.FromString<TestDocument>(filter);
            }
        }

        private static List<string> loadFilters()
        {
            return new List<string>
            {
                "Value eq 3",
                "value EQ 3",
                "value eq 3 ",
                "subdocument.value eq 300",
                "subdocument.value ge 300 and subdocument.value lt 999",
                "subdocument.subdocument.value eq 300",
                "value gt 3",
                "value ge 7",
                "value lt 8",
                "value le 3",
                "value ne 3",
                "value eq 3",
                "value gt 3 and value le 5",
                "value gt 3 and value le 5 or value le 1",
                "not value eq 3",
                "not value gt 7",
                "not value eq 3 and value lt 4",
                "value eq 3 or value eq 5 and value gt 4",
                "(value eq 3 or value eq 5) and value gt 4",
                "((value eq 3 or value eq 5) and value gt 4) or value eq 1",
                "anothervalue eq 1 and (value eq 3 or value eq 4)",
                "text eq 'Item 7'",
                "moretext eq Item7",
                "text eq 'Item 7' or text eq 'Item 2'",
                "value gt 6 and value le 9 or text eq 'Item 2'",
                "text eq 'Item 7' and value gt 5",
                "nullablevalue eq null",
                "not nullablevalue eq null",
                "not nullablevalue eq null and value gt 5"
            };
        }
    }
}
