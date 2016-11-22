using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlFilter.Tests
{
    [TestFixture]
    public class QueryStringTests
    {
        [Test]
        public void should_match_one_document_with_eq_expression()
        {
            var expectedValue = 3;
            var queryString = $"Value eq {expectedValue}";
            var testDocs = GetTestDocuments(10);
            
            var expression = QueryString<TestDocument>.GetWhereExpression(queryString);
            var result = testDocs.Where(expression);

            foreach (var doc in result)
            {
                doc.Value.Should().Be(expectedValue);
            }
        }

        private IQueryable<TestDocument> GetTestDocuments(int quantity)
        {
            return Enumerable.Range(0, quantity)
                .Select(x => new TestDocument { Value = x, Text = $"Item{x}" })
                .AsQueryable();
        }
    }
}
