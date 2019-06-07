using FluentAssertions;
using System;
using System.Linq.Expressions;
using UrlFilter.ExpressionProcessors;
using UrlFilter.ExpressionReducers;
using Xunit;

namespace UrlFilter.Tests
{
    public class UnaryProcessorTests
    {
        [Fact]
        public void should_invert_expression_with_not()
        {
            var testDoc = new TestDocument { Value = 4 };            
            var param = Expression.Parameter(typeof(TestDocument));
            var expression = generateComparisonReducer().ReduceComparison("value", "eq", "5", param); ;
            var sut = new UnaryProcessor();

            var unaryExpression = sut.Process("not", expression);
            var lambda = Expression.Lambda<Func<TestDocument, bool>>(unaryExpression, param).Compile();
            var result = lambda(testDoc);

            result.Should().BeTrue();
        }

        private ComparisonReducer generateComparisonReducer()
        {
            var propInfo = new PropertyInfoProvider();
            var propExpFac = new PropertyExpressionFactory(propInfo);
            return new ComparisonReducer(
                new ComparisonProcessor(),
                new PropertyProcessor(propInfo, new PropertyExpressionFactory(propInfo)),
                new ValueProcessor(),
                new NotNullExpressionProcessor(propExpFac)
                );
        }
    }
}
