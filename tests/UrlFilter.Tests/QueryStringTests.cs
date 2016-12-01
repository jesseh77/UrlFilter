using System;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace UrlFilter.Tests
{
    public class QueryStringTests
    {
        [Theory(DisplayName = "Inputs")]
        [InlineData(10, 3, "Value eq 3")]
        [InlineData(10, 3, "value EQ 3")]
        [InlineData(10, 3, "value eq 3 ")]
        [InlineData(10, 3, "subdocument.value eq 300")]
        public void should_handle_string_anomalies(int qty, int expected, string query)
        {
            var testDocs = GetTestDocuments(qty);
            
            var expression = QueryString<TestDocument>.GetWhereExpression(query);
            var result = testDocs.Where(expression).ToList();

            foreach (var doc in result)
            {
                doc.Value.Should().Be(expected);
            }
        }

        [Theory(DisplayName = "Filters")]
        [InlineData(10, 7, "value gt 3")]
        [InlineData(10, 4, "value ge 7")]
        [InlineData(10, 7, "value lt 8")]
        [InlineData(10, 3, "value le 3")]
        [InlineData(10, 9, "value ne 3")]
        [InlineData(10, 1, "value eq 3")]
        [InlineData(10, 2, "value gt 3 and value le 5")]
        [InlineData(10, 3, "value gt 3 and value le 5 or value le 1")]
        [InlineData(10, 9, "not value eq 3")]
        [InlineData(10, 7, "not value gt 7")]
        [InlineData(10, 2, "not value eq 3 and value lt 4")]
        [InlineData(10, 1, "value eq 3")]
        [InlineData(10, 1, "text eq 'Item7'")]
        [InlineData(10, 1, "text eq Item7")]
        [InlineData(10, 2, "text eq 'Item7' or text eq 'Item2'")]
        [InlineData(10, 4, "value gt 6 and value le 9 or text eq 'Item2'")]
        [InlineData(10, 1, "text eq 'Item7' and value gt 5")]
        public void should_return_match_count(int qty, int expected, string query)
        {
            var testDocs = GetTestDocuments(qty);

            var expression = QueryString<TestDocument>.GetWhereExpression(query);
            var result = testDocs.Where(expression).ToList();

            result.Count.Should().Be(expected);
        }

        private static IQueryable<TestDocument> GetTestDocuments(int quantity)
        {
            return Enumerable.Range(1, quantity)
                .Select(x => new TestDocument { Value = x, Text = $"Item{x}",
                    SubDocument = new TestDocument { Value = x * 100, Text = $"Item{x * 100 }" } })
                    .AsQueryable();
        }
    }
}
