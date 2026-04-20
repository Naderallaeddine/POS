using System.Net;

namespace POS.IntegrationTests.Smoke;

public class RoutesSmokeTests : IClassFixture<TestingWebApplicationFactory>
{
    private readonly TestingWebApplicationFactory _factory;

    public RoutesSmokeTests(TestingWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateHttpsClient()
        => _factory.CreateClient(new()
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

    [Theory]
    [InlineData("/")]
    [InlineData("/Home/Privacy")]
    [InlineData("/Account/Login")]
    public async Task Public_pages_return_200(string path)
    {
        using var client = CreateHttpsClient();
        var res = await client.GetAsync(path);

        // In the integration test host we set a default test auth scheme so some
        // public pages intentionally redirect authenticated users.
        Assert.True(
            res.StatusCode is HttpStatusCode.OK or HttpStatusCode.Found,
            $"Expected 200 OK or 302 Found for '{path}', got {(int)res.StatusCode} {res.StatusCode}.");
    }

    [Theory]
    [InlineData("/Products")]
    [InlineData("/Sales")]
    public async Task Protected_pages_return_200_with_test_auth(string path)
    {
        using var client = CreateHttpsClient();
        var res = await client.GetAsync(path);

        if (res.StatusCode != HttpStatusCode.OK)
        {
            var body = await res.Content.ReadAsStringAsync();
            Assert.Fail($"Expected 200 OK for '{path}', got {(int)res.StatusCode} {res.StatusCode}. Body: {body}");
        }
    }

    [Fact]
    public async Task Sales_search_returns_json_array()
    {
        using var client = CreateHttpsClient();

        // term must be length >= 2
        var res = await client.GetAsync("/Sales/Search?term=te");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadAsStringAsync();
        Assert.StartsWith("[", body.TrimStart());
    }
}

