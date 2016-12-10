using FluentAssertions;
using System.Linq;
using Xunit;

namespace UrlFilter.Tests
{
    public class QueryStringTests
    {
        [Theory(DisplayName = "Filters")]
        [InlineData(10, "Value eq 3", new [] {3})]
        [InlineData(10, "value EQ 3", new[] { 3 })]
        [InlineData(10, "value eq 3 ", new[] { 3 })]
        [InlineData(10, "subdocument.value eq 300", new[] { 3 })]
        [InlineData(10, "subdocument.value ge 300 and subdocument.value lt 999", new[] { 3, 4, 5, 6, 7, 8, 9 })]
        [InlineData(10, "subdocument.subdocument.value eq 300", new int[] { })]
        [InlineData(10, "value gt 3", new [] {4,5,6,7,8,9,10})]
        [InlineData(10, "value ge 7", new [] {7,8,9,10})]
        [InlineData(10, "value lt 8", new [] {1,2,3,4,5,6,7})]
        [InlineData(10, "value le 3", new [] {1,2,3})]
        [InlineData(10, "value ne 3", new [] {1,2,4,5,6,7,8,9,10})]
        [InlineData(10, "value eq 3", new [] {3})]
        [InlineData(10, "value gt 3 and value le 5", new [] {4,5})]
        [InlineData(10, "value gt 3 and value le 5 or value le 1", new [] {1,4,5})]
        [InlineData(10, "not value eq 3", new [] {1,2,4,5,6,7,8,9,10})]
        [InlineData(10, "not value gt 7", new [] {1,2,3,4,5,6,7})]
        [InlineData(10, "not value eq 3 and value lt 4", new [] {1,2})]
        [InlineData(10, "value eq 3 or value eq 5 and value gt 4", new [] {3,5})]
        [InlineData(10, "(value eq 3 or value eq 5) and value gt 4", new [] {5})]
        [InlineData(10, "text eq 'Item7'", new [] {7})]
        [InlineData(10, "text eq Item7", new [] {7})]
        [InlineData(10, "text eq 'Item7' or text eq 'Item2'", new [] {2,7})]
        [InlineData(10, "value gt 6 and value le 9 or text eq 'Item2'", new [] {2,7,8,9})]
        [InlineData(10, "text eq 'Item7' and value gt 5", new [] {7})]
        public void should_return_match_count(int qty, string query, int[] expectedValues)
        {
            var testDocs = GetTestDocuments(qty);

            var expression = WhereExpression.Build.FromString<TestDocument>(query);
            var result = testDocs.Where(expression).ToList();

            result.Select(x => x.Value).Should().BeEquivalentTo(expectedValues);
        }

        //[Theory(DisplayName ="Exceptions")]
        //public void should_throw_exception(int qty, string query)
        //{
        //    var testDocs = GetTestDocuments(qty);

        //    var expression = WhereExpression.Build.FromString<TestDocument>(query);
            
        //}

        private static IQueryable<TestDocument> GetTestDocuments(int quantity)
        {
            return Enumerable.Range(1, quantity)
                .Select(x => new TestDocument { Value = x, Text = $"Item{x}",
                    SubDocument = new TestDocument { Value = x * 100, Text = $"Item{x * 100 }" } })
                    .AsQueryable();
        }
    }
}
