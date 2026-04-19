using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using POS.Authorization;
using POS.Configuration;
using POS.Data.Entities;
using POS.Interfaces.Services;

namespace POS.Services;

public class IdentityDataSeeder : IIdentityDataSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SeedOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<IdentityDataSeeder> _logger;

    public IdentityDataSeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IOptions<SeedOptions> options,
        IHostEnvironment environment,
        ILogger<IdentityDataSeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _options = options.Value;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_environment.IsDevelopment())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.AdminEmail) || string.IsNullOrWhiteSpace(_options.AdminPassword))
        {
            return;
        }

        var admin = await _userManager.FindByEmailAsync(_options.AdminEmail);
        if (admin is not null)
        {
            return;
        }

        admin = new ApplicationUser
        {
            UserName = _options.AdminEmail,
            Email = _options.AdminEmail,
            EmailConfirmed = true,
            DisplayName = "Administrator"
        };

        var result = await _userManager.CreateAsync(admin, _options.AdminPassword);
        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Development admin user was not created: {Errors}",
                string.Join("; ", result.Errors.Select(e => e.Description)));
            return;
        }

        await _userManager.AddToRoleAsync(admin, AppRoles.Admin);
        _logger.LogInformation("Development admin user {Email} was created.", _options.AdminEmail);
    }
}
