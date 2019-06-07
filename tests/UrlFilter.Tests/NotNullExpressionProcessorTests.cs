using FluentAssertions;
using System;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;
using Xunit;

namespace UrlFilter.Tests
{
    public class NotNullExpressionProcessorTests
    {
        [Fact]
        public void should_short_circuit_expression_to_false_on_null_object()
        {
            var testDoc = new TestDocument { SubDocument = new TestDocument() };
            var path = "subdocument.subdocument.value";
            var sut = new NotNullExpressionProcessor(new PropertyExpressionFactory(new PropertyInfoProvider()));
            var paramExpression = Expression.Parameter(typeof(TestDocument));

            var notNullExpression = sut.NotNullPropertyExpression(path, paramExpression);
            var lambda = Expression.Lambda<Func<TestDocument, bool>>(notNullExpression, paramExpression).Compile();
            var result = lambda(testDoc);

            result.Should().BeFalse();
        }
    }
}
