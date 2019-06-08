using System;
using FluentAssertions;
using System.Linq;
using System.Linq.Expressions;
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
        [InlineData(10, "value eq 3 or value eq 5 and value gt 4", new [] {5})]
        [InlineData(10, "(value eq 3 or value eq 5) and value gt 4", new [] {5})]
        [InlineData(10, "((value eq 3 or value eq 5) and value gt 4) or value eq 1", new[] { 1, 5 })]
        [InlineData(10, "anothervalue eq 1 and (value eq 3 or value eq 4)", new[] { 4 })]
        [InlineData(10, "text eq 'Item 7'", new [] {7})]
        [InlineData(10, "moretext eq Item7", new [] {7})]
        [InlineData(10, "text eq 'Item 7' or text eq 'Item 2'", new [] {2,7})]
        [InlineData(10, "value gt 6 and value le 9 or text eq 'Item 2'", new [] {2,7,8,9})]
        [InlineData(10, "text eq 'Item 7' and value gt 5", new [] {7})]
        [InlineData(10, "nullablevalue eq null", new[] { 2, 4, 6, 8, 10 })]
        [InlineData(10, "not nullablevalue eq null", new[] { 1, 3, 5, 7, 9 })]
        [InlineData(10, "not nullablevalue eq null and value gt 5", new[] { 7, 9 })]
        public void should_return_match_count(int qty, string query, int[] expectedValues)
        {
            var testDocs = GetTestDocuments(qty);
            var expression = WhereExpression.Build.FromString<TestDocument>(query);
            
            var result = testDocs.Where(expression).ToList();

            result.Select(x => x.Value).Should().BeEquivalentTo(expectedValues);
        }

        [InlineData("value eq 5 or )text eq 'Item7')")]
        [InlineData("(value eq 5")]
        [InlineData("value eq 5)")]
        [InlineData("((value eq 5)")]
        [InlineData("(value eq 5))")]
        [Theory(DisplayName ="Query text validation")]
        public void should_throw_exception(string query)
        {
            Action action = () => WhereExpression.Build.FromString<TestDocument>(query);

            Assert.Throws<QueryStringException>(action);
        }

        [Fact(Skip = "not yet implemented")]
        public void should_create_expression_from_existing_expression()
        {
            Expression<Func<TestDocument, bool>> expression = x => x.Value > 3;
            var filter = "value lt 7";
            var testDocs = GetTestDocuments(10);
            var expected = new[] { 4, 5, 6 };

            var resultExpression = WhereExpression.Build.FromString(filter, expression);
            var result = testDocs.Where(resultExpression).ToList();

            result.Select(x => x.Value).Should().BeEquivalentTo(expected);
        }

        private static IQueryable<TestDocument> GetTestDocuments(int quantity)
        {
            return Enumerable.Range(1, quantity)
                .Select(x => new TestDocument(x, true))
                    .AsQueryable();
        }
    }
}
