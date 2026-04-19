using POS.ViewModels.Dashboard;

namespace POS.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default);
}

