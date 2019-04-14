using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using UrlFilter.ExpressionProcessors;
using Xunit;

namespace UrlFilter.Tests
{
    public class ValueProcessorTests
    {
        [Fact]
        public void should_create_int_const_expression()
        {
            var value = 7;
            var propertyInfo = new PropertyInfoProvider().GetPropertyInfoFromName("Value", typeof(TestDocument));
            var sut = new ValueProcessor();

            var result = sut.Process(value.ToString(), propertyInfo);

            result.Value.Should().Be(value);
        }

        [Fact]
        public void should_create_string_const_expression()
        {
            var value = "Some text";
            var propertyInfo = new PropertyInfoProvider().GetPropertyInfoFromName("Text", typeof(TestDocument));
            var sut = new ValueProcessor();

            var result = sut.Process(value, propertyInfo);

            result.Value.Should().Be(value);
        }

        [Fact]
        public void should_create_date_const_expression()
        {
            var value = "2018-05-15";
            var propertyInfo = new PropertyInfoProvider().GetPropertyInfoFromName("ADate", typeof(TestDocument));
            var sut = new ValueProcessor();

            var result = sut.Process(value, propertyInfo);

            result.Value.Should().Be(DateTime.Parse(value));
        }

        [Fact]
        public void should_throw_for_object_const_expression()
        {
            var propertyInfo = new PropertyInfoProvider().GetPropertyInfoFromName("SubDocument", typeof(TestDocument));
            var sut = new ValueProcessor();

            sut.Invoking(x => x.Process("value", propertyInfo))
                .Should().Throw<NotImplementedException>();            
        }

        [Fact]
        public void should_throw_for_collection_const_expression()
        {
            var propertyInfo = new PropertyInfoProvider().GetPropertyInfoFromName("DocumentCollection", typeof(TestDocument));
            var sut = new ValueProcessor();

            sut.Invoking(x => x.Process("value", propertyInfo))
                .Should().Throw<NotImplementedException>();
        }
    }
}
