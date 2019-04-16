using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using UrlFilter.ExpressionProcessors;
using Xunit;

namespace UrlFilter.Tests
{
    public class LogicalProcessorTests
    {
        [Theory]
        [InlineData("gt")]
        [InlineData("ge")]
        [InlineData("lt")]
        [InlineData("le")]
        [InlineData("eq")]
        [InlineData("ne")]
        public void should_return_can_process_true_for_valid_compairson_types(string comparisonType)
        {
            var sut = new ComparisonProcessor();

            var result = sut.CanProcess(comparisonType);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Gt")]
        [InlineData("gE")]
        [InlineData("LT")]
        [InlineData("Le")]
        [InlineData("eQ")]
        [InlineData("NE")]
        public void should_return_can_process_true_for_mixed_case_compairson_types(string comparisonType)
        {
            var sut = new ComparisonProcessor();

            var result = sut.CanProcess(comparisonType);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("and")]
        [InlineData("or")]
        [InlineData("not")]
        [InlineData("bill")]
        public void should_return_can_process_false_for_non_comparison_types(string comparisonType)
        {
            var sut = new ComparisonProcessor();

            var result = sut.CanProcess(comparisonType);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 2, "gt", false)]
        [InlineData(2, 2, "ge", true)]
        [InlineData(1, 2, "lt", true)]
        [InlineData(3, 2, "le", false)]
        [InlineData(2, 2, "eq", true)]
        [InlineData(2, 2, "ne", false)]
        public void should_create_expression_that_uses_provided_comparison_operator(int val1, int val2, string comparisonType, bool expectedResult)
        {
            var exp1 = Expression.Constant(val1);
            var exp2 = Expression.Constant(val2);

            var sut = new ComparisonProcessor();
            var expression = sut.Process(comparisonType, exp1, exp2);
            var lambda = Expression.Lambda<Func<bool>>(expression).Compile();

            var result = lambda();

            result.Should().Be(expectedResult);
        }
    }
}
