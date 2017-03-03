using FluentAssertions;
using Nancy;
using Nancy.Testing;
using Xunit;

namespace DemoApi.Tests
{
    public class FilterDataTests
    {
        [Fact]
        public void should_get_100_people_for_empty_filter()
        {
            var response = Get();
            var result = response.Body.DeserializeJson<FilterResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            result.FilterText.Should().Be(string.Empty);
            result.People.Count.Should().Be(100);
        }

        [InlineData("id gt 98", 1)]
        [InlineData("id gt 88 and id le 91", 3)]
        [InlineData("(id lt 8 and id ge 2) or (id gt 77 and id le 79)", 8)]
        [InlineData("not id gt 9 or id ge 91", 19)]
        [Theory]
        public void should_get_filtered_result(string filter, int expectedQuantity)
        {
            var response = Get(filter);
            var result = response.Body.DeserializeJson<FilterResponse>();

            result.FilterText.Should().Be(filter);
            result.People.Count.Should().Be(expectedQuantity);
        }

        private BrowserResponse Get(string filter = null)
        {
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            return browser.Get("/filterData", with => {
                with.Accept("application/json");
                with.Query("$filter", filter);
                with.HttpRequest();
            });
        }
    }
}
