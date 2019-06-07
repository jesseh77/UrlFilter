using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using UrlFilter.ExpressionProcessors;
using UrlFilter.ExpressionReducers;
using Xunit;

namespace UrlFilter.Tests
{
    public class ComparisonReducerTests
    {
        [Theory]
        [InlineData("Value", "eq", "5", "5", true)]
        [InlineData("Value", "eq", "5", "6", false)]
        [InlineData("Value", "ne", "5", "5", false)]
        [InlineData("Value", "ne", "5", "6", true)]
        [InlineData("Value", "gt", "5", "6", true)]
        [InlineData("Value", "gt", "6", "5", false)]
        [InlineData("Value", "ge", "5", "6", true)]
        [InlineData("Value", "ge", "6", "5", false)]
        [InlineData("Value", "lt", "6", "5", true)]
        [InlineData("Value", "le", "6", "6", true)]
        [InlineData("Text", "ne", "some text", "some other text", true)]
        [InlineData("Text", "ne", "some text", "some text", false)]
        [InlineData("Text", "eq", "some text", "some text", true)]
        [InlineData("Text", "eq", "some text", "some other text", false)]
        public void should_create_valid_expression_from_value_property_and_comparison(string leftValue, string comparisonOperator, string rightValue, string propValue, bool expectedResult)
        {
            var paramExpression = Expression.Parameter(typeof(TestDocument));
            var testDoc = createTestDoc(leftValue, propValue);
            var sut = constructComparisonReducer();

            var expression = sut.ReduceComparison(leftValue, comparisonOperator, rightValue, paramExpression);
            var exp = Expression.Lambda<Func<TestDocument, bool>>(expression,paramExpression).Compile();
            var result = exp(testDoc);

            result.Should().Be(expectedResult);
        }

        private TestDocument createTestDoc(string propName, string value)
        {
            var testDoc = new TestDocument();
            var prop = typeof(TestDocument).GetProperty(propName);
            var propValue = Convert.ChangeType(value, prop.PropertyType);
            prop.SetValue(testDoc, propValue);
            return testDoc;
        }

        private ComparisonReducer constructComparisonReducer()
        {
            var propInfo = new PropertyInfoProvider();
            return new ComparisonReducer(
                new ComparisonProcessor(),
                new PropertyProcessor(propInfo, new PropertyExpressionFactory(propInfo)),
                new ValueProcessor()
                );
        }
    }
}
