using Hospital.Application.Abstractions;
using Hospital.Application.Orders.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hospital.Infrastructure.BackgroundJobs
{
    public class OrderProcessingWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderProcessingWorker> _logger;

        public OrderProcessingWorker(IServiceScopeFactory scopeFactory, ILogger<OrderProcessingWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service Worker de órdenes iniciado");

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<ProcessPendingOrdersCommand, int>>();

                    var processed = await handler.Handle(new ProcessPendingOrdersCommand());

                    if (processed > 0)
                        _logger.LogInformation("{Count} orden(es) procesada(s)", processed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando órdenes");
                }
            }
        }
    }
}