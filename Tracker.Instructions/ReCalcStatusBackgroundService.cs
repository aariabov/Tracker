namespace Tracker.Instructions;

public class ReCalcStatusBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ReCalcStatusBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var hoursBeforeStart = 24 - DateTime.Now.Hour;
        await Task.Delay(TimeSpan.FromHours(hoursBeforeStart), stoppingToken);

        // TODO: сделать блокировку, чтоб был активный только один инстанс
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var reCalcStatusService = scope.ServiceProvider.GetRequiredService<ReCalcStatusService>();
                await reCalcStatusService.RecalculateStatusesForRootInWorkAndDeadlineLessNow();
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
