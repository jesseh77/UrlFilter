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
        [InlineData("(value gt 3) and (value le 5 or value ne 1)", true)]
        [InlineData("(value gt 3) and value le 5", true)]
        [InlineData("value gt 3 and (value le 5)", true)]
        [InlineData("value gt 3 and value le 5", true)]
        [InlineData("value eq 4 or value eq 5", true)]
        [InlineData("(value gt 3 and value le 5)", true)]
        [InlineData("(value gt 3 and (value le 5))", true)]
        [InlineData("((value gt 3) and (value le 5))", true)]
        [InlineData("not value lt 3", true)]
        [InlineData("value gt 6 or value le 9 and value eq 4", true)]
        [InlineData("value gt 3 and value ge 5 or value le 1", false)]
        [InlineData("((value gt 3) and not (value eq 5))", true)]
        [InlineData("(value gt 3 and (value le 5)) and text eq 'some text'", true)]
        [InlineData("((value gt 3) and (value le 5)) and text ne 'some other text'", true)]
        public void should_reduce_simple_and_operator(string queryText, bool expected)
        {
            var testDoc = new TestDocument { Value = 4, Text = "some text" };
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

            var expression = sut.ProcessBlock(queryText, paramExpression, null, "and");
            var lambda = Expression.Lambda<Func<TestDocument, bool>>(expression, paramExpression).Compile();
            var result = lambda(testDoc);

            result.Should().BeTrue();
        }

        [Fact]
        public void should_process_expression_block_with_supplied_left_expression()
        {
            var testDoc = new TestDocument { Value = 4 };
            var testQuery = "value le 5";
            var sut = generateLogicalReducer();
            var paramExpression = Expression.Parameter(typeof(TestDocument));
            var leftExpression = sut.ReduceLogical(testQuery,paramExpression);            
            var queryText = "value gt 3";            

            var expression = sut.ProcessBlock(queryText, paramExpression, leftExpression, "and");
            var lambda = Expression.Lambda<Func<TestDocument, bool>>(expression, paramExpression).Compile();
            var result = lambda(testDoc);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("value eq 5", "value-eq-5")]
        [InlineData("value eq 5 ", "value-eq-5")]
        [InlineData("value eq 'five'", "value-eq-five")]
        [InlineData("value eq 'five oh seven'", "value-eq-five oh seven")]
        public void should_split_segment_on_space_and_single_quote(string block, string expected)
        {
            var sut = generateLogicalReducer();

            var result = string.Join('-', sut.splitSegments(block));

            result.Should().Be(expected);
        }

        private LogicalReducer generateLogicalReducer()
        {
            var propInfo = new PropertyInfoProvider();
            return new LogicalReducer(
                new ComparisonReducer(
                    new ComparisonProcessor(),
                    new PropertyProcessor(propInfo, new PropertyExpressionFactory(propInfo)),
                    new ValueProcessor()),
                new UnaryProcessor(),
                new LogicalProcessor());
        }
    }
}