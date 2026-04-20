using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace POS.PlaywrightTests;

public class LoginPageSmokeTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
        => new()
        {
            IgnoreHTTPSErrors = true,
        };

    [Test]
    public async Task LoginPage_RendersSignInForm()
    {
        var baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL")?.Trim();
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            Assert.Ignore("Set E2E_BASE_URL to run Playwright E2E tests (e.g. https://localhost:5271).");
            return;
        }

        await Page.GotoAsync($"{baseUrl.TrimEnd('/')}/Account/Login", new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
        });

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Sign in" })).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name=\"Email\"]")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name=\"Password\"]")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Sign in" })).ToBeEnabledAsync();
    }
}
