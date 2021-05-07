using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Server.Tools
{
    public class ReloadDataService : IHostedService, IDisposable
    {

        private readonly IServiceProvider services;
        private readonly Settings settings;
        private readonly DataStorage dataStorage;

        private Timer timer;

        public ReloadDataService(IServiceProvider services)
        {
            this.services = services;
            this.settings = this.services.GetRequiredService<Settings>();
            this.dataStorage = this.services.GetRequiredService<DataStorage>();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            int runInterval = settings.RunInterval;

            timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(runInterval));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            dataStorage.LoadData();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
