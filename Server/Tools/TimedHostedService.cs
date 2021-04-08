using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Server.Tools
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private Timer timer;

        public Task StartAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(10));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            DataStorage.LoadData();
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
