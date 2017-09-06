using DemoApi.data;
using FluentAssertions;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;
using Xunit;

namespace DemoApi.Tests
{
    public class FilterDataTests
    {
        [Fact]
        public void should_get_888_people_for_empty_filter()
        {
            var response = Get();
            var result = response.Body.DeserializeJson<FilterResponse<HockeyStat>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            result.FilterText.Should().Be(string.Empty);
            result.Values.Count.Should().Be(888);
        }

        [InlineData("firstName eq 'Sidney'", 1)]
        [InlineData("draftRound eq 1 and draftPick le 5", 5)]
        [InlineData("(playerId lt 8 and playerId ge 2) or (playerId gt 77 and playerId le 79)", 8)]
        [InlineData("not playerId gt 9 or playerId ge 91", 19)]
        [Theory]
        public void should_get_filtered_result(string filter, int expectedQuantity)
        {
            var response = Get(filter);
            var result = JsonConvert.DeserializeObject<FilterResponse<HockeyStat>>(response.Body.AsString());

            result.FilterText.Should().Be(filter);
            result.Values.Count.Should().Be(expectedQuantity);
        }

        private BrowserResponse Get(string filter = null)
        {
            var bootstrapper = new Bootstrapper();
            var browser = new Browser(bootstrapper);

            return browser.Get("/filterData", with => {
                with.Accept("application/json");
                with.Query("$filter", filter);
                with.HttpRequest();
            });
        }
    }
}
