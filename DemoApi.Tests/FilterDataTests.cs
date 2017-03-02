using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.Testing;
using Xunit;

namespace DemoApi.Tests
{
    public class FilterDataTests
    {
        [Fact]
        public void should_get_200_people_for_empty_filter()
        {
            var response = Get();
            var result = response.Body.DeserializeJson<FilterResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            result.FilterText.Should().Be(null);
            result.People.Count.Should().Be(100);
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
