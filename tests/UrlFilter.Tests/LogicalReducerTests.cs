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
    public class LogicalReducerTests
    {        
        [Theory]
        [InlineData("(value gt 3) and (value le 5)", true)]
        [InlineData("(value gt 3) and value le 5", true)]
        [InlineData("value gt 3 and (value le 5)", true)]
        [InlineData("value gt 3 and value le 5", true)]
        [InlineData("(value gt 3 and value le 5)", true)]
        [InlineData("(value gt 3 and (value le 5))", true)]
        [InlineData("((value gt 3) and (value le 5))", true)]
        public void should_reduce_simple_and_operator(string queryText, bool expected)
        {
            var testDoc = new TestDocument { Value = 4 };
            var sut = generateLogicalReducer();
            var paramExpression = Expression.Parameter(typeof(TestDocument));

            var expression = sut.ReduceLogical(queryText, paramExpression);
            var lambda = Expression.Lambda<Func<TestDocument, bool>>(expression, paramExpression).Compile();
            var result = lambda(testDoc);

            result.Should().Be(expected);
        }

        [Fact]
        public void should_process_expression_block_without_brackets()
        {
            var testDoc = new TestDocument { Value = 4 };
            var queryText = "value gt 3 and value le 5";
            var sut = generateLogicalReducer();
            var paramExpression = Expression.Parameter(typeof(TestDocument));

            var expression = sut.ProcessBlock(queryText, paramExpression, Expression.Empty(), "and");
            var lambda = Expression.Lambda<Func<TestDocument, bool>>(expression, paramExpression).Compile();
            var result = lambda(testDoc);

            result.Should().BeTrue();
        }

        [Fact]
        public void should_process_expression_block_with_supplied_left_expression()
        {
            var testDoc = new TestDocument { Value = 4 };
            Expression<Func<TestDocument, bool>> exp = x => x.Value <= 5;
            var paramExpression = Expression.Parameter(typeof(TestDocument));
            var leftExpression = Expression.Lambda(exp, paramExpression);
            
            var queryText = "value gt 3";
            var sut = generateLogicalReducer();

            var expression = sut.ProcessBlock(queryText, paramExpression, leftExpression, "and");
            var lambda = Expression.Lambda<Func<TestDocument, bool>>(expression, paramExpression).Compile();
            var result = lambda(testDoc);

            result.Should().BeTrue();
        }

        private LogicalReducer generateLogicalReducer()
        {
            return new LogicalReducer(
                new ComparisonReducer(
                    new ComparisonProcessor(),
                    new PropertyProcessor(new PropertyInfoProvider()),
                    new ValueProcessor()),
                new UnaryProcessor(),
                new LogicalProcessor());
        }
    }
}