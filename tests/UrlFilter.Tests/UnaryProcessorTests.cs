using FluentAssertions;
using System;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;
using Xunit;

namespace UrlFilter.Tests
{
    public class UnaryProcessorTests
    {
        [Fact]
        public void should_invert_expression_with_not()
        {
            var testDoc = new TestDocument { Value = 4 };
            Expression<Func<TestDocument, bool>> expression = x => x.Value == 5;
            var sut = new UnaryProcessor();

            var unaryExpression = sut.Process("not", expression);
            var lambda = Expression.Lambda<Func<TestDocument, bool>>(expression, expression.Parameters[0]).Compile();
            var result = lambda(testDoc);

            result.Should().BeTrue();
        }
    }
}
