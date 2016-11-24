using System;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace UrlFilter.Tests
{
    public class QueryStringTests
    {
        [Theory]
        [InlineData(10, 3, "Value eq 3")]
        [InlineData(10, 3, "value EQ 3")]
        [InlineData(10, 3, "value eq 3 ")]
        public void should_handle_string_anomalies(int testDocQuantity, int expectedValue, string filterString)
        {
            var testDocs = GetTestDocuments(testDocQuantity);
            
            var expression = QueryString<TestDocument>.GetWhereExpression(filterString);
            var result = testDocs.Where(expression);

            foreach (var doc in result)
            {
                doc.Value.Should().Be(expectedValue);
            }
        }

        [Theory]
        [InlineData(10, 7, "value gt 3")]
        [InlineData(10, 4, "value ge 7")]
        [InlineData(10, 7, "value lt 8")]
        [InlineData(10, 3, "value le 3")]
        [InlineData(10, 9, "value ne 3")]
        [InlineData(10, 1, "value eq 3")]
        [InlineData(10, 2, "value gt 3 and value le 5")]
        [InlineData(10, 3, "value gt 3 and value le 5 or value le 1")]
        [InlineData(10, 1, "value eq 3")]
        [InlineData(10, 1, "value eq 3")]
        [InlineData(10, 1, "value eq 3")]
        [InlineData(10, 1, "value eq 3")]
        [InlineData(10, 1, "text eq 'Text7'")]
        [InlineData(10, 2, "text eq 'Text7' or text eq 'Text2'")]
        [InlineData(10, 2, "text eq 'Text7' and value gt 5")]
        public void should_return_match_count(int testDocQuantity, int expectedCount, string filterString)
        {
            var testDocs = GetTestDocuments(testDocQuantity);

            var expression = QueryString<TestDocument>.GetWhereExpression(filterString);
            var result = testDocs.Where(expression);

            result.Count().Should().Be(expectedCount);
        }

        private static IQueryable<TestDocument> GetTestDocuments(int quantity)
        {
            return Enumerable.Range(1, quantity)
                .Select(x => new TestDocument { Value = x, Text = $"Item{x}" })
                .AsQueryable();
        }
    }
}
