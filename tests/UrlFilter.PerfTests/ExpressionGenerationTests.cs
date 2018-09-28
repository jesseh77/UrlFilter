using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UrlFilter.PerfTests
{
    public class ExpressionGenerationTests
    {
        private static WhereExpression expressionProcessor = new WhereExpression();

        [Benchmark]
        public Expression GenerateExpressions()
        {
            return expressionProcessor.FromString<TestDocument>(Filter);
        }

        [ParamsSource(nameof(Filters))]
        private string Filter { get; set; }
        private static IEnumerable<string> Filters()
        {
            return new string[]
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
