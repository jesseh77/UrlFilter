using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using UrlFilter.ExpressionProcessors;
using Xunit;

namespace UrlFilter.Tests
{
    public class PropertyProcessorTests
    {
        [Fact]
        public void should_return_canprocess_true_for_valid_property_name()
        {
            var propertyName = nameof(TestDocument.Value);
            var sut = new PropertyProcessor();
            var parameter = Expression.Parameter(typeof(TestDocument));

            var result = sut.CanProcess(propertyName, parameter);

            result.Should().BeTrue();
        }

        [Fact]
        public void should_return_canprocess_false_for_invalid_property_name()
        {
            var propertyName = "invalidName";
            var sut = new PropertyProcessor();
            var parameter = Expression.Parameter(typeof(TestDocument));

            var result = sut.CanProcess(propertyName, parameter);

            result.Should().BeFalse();
        }

        [Fact]
        public void should_ignore_case_when_validating_property_names()
        {
            var propertyName = nameof(TestDocument.Value).ToUpper();
            var sut = new PropertyProcessor();
            var parameter = Expression.Parameter(typeof(TestDocument));

            var result = sut.CanProcess(propertyName, parameter);

            result.Should().BeTrue();
        }

        [Fact]
        public void should_validate_child_property_name()
        {
            var propertyName = $"{nameof(TestDocument.SubDocument)}.{nameof(TestDocument.SubDocument.Value)}";
            var sut = new PropertyProcessor();
            var parameter = Expression.Parameter(typeof(TestDocument));

            var result = sut.CanProcess(propertyName, parameter);

            result.Should().BeTrue();
        }

        [Fact]
        public void should_return_false_for_invalid_child_property_name()
        {
            var propertyName = $"{nameof(TestDocument.SubDocument)}.invalidName";
            var sut = new PropertyProcessor();
            var parameter = Expression.Parameter(typeof(TestDocument));

            var result = sut.CanProcess(propertyName, parameter);

            result.Should().BeFalse();
        }

        [Fact]
        public void should_return_false_for_invalid_parent_property_name()
        {
            var propertyName = $"invalidName.{nameof(TestDocument.SubDocument.Value)}";
            var sut = new PropertyProcessor();
            var parameter = Expression.Parameter(typeof(TestDocument));

            var result = sut.CanProcess(propertyName, parameter);

            result.Should().BeFalse();
        }
    }
}
