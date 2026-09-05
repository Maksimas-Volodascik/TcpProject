using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpListenerProject.Entity;

namespace TcpListenerProject.Logging
{
    public class LogWriter : BackgroundService
    {
        private const int MaxBatch = 500;
        private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(300);
        private static readonly TimeSpan CancellationTimeout = TimeSpan.FromSeconds(5);

        private readonly LogQueue _queue;
        private readonly IDbContextFactory<DataContext> _factory;

        public LogWriter(LogQueue queue, IDbContextFactory<DataContext> factory)
        {
            _queue = queue;
            _factory = factory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var reader = _queue.Reader;

            while (!stoppingToken.IsCancellationRequested)
            {
                var batch = new List<LogEntry>(MaxBatch);

                using var window = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                window.CancelAfter(Interval);

                try
                {
                    while (batch.Count < MaxBatch && await reader.WaitToReadAsync(window.Token))
                    {
                        while (batch.Count < MaxBatch && reader.TryRead(out var entry))
                        {
                            batch.Add(entry);
                        }
                    }
                }
                catch (OperationCanceledException err) when (!stoppingToken.IsCancellationRequested) { }

                if (batch.Count > 0)
                    await FlushAsync(batch, stoppingToken);
            }

            var remaining = new List<LogEntry>(MaxBatch);
            using var shutdownTokenSource = new CancellationTokenSource(CancellationTimeout);
            while (reader.TryRead(out var entry)) //Read logs from the channel after shutdown
            {
                remaining.Add(entry);
                if (remaining.Count >= MaxBatch)
                {
                    await FlushOnceAsync(remaining, shutdownTokenSource.Token);
                    remaining.Clear();
                }
            }

            if (remaining.Count > 0)
                await FlushOnceAsync(remaining, shutdownTokenSource.Token);
        }

        private async Task FlushOnceAsync(List<LogEntry> batch, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Flush: {batch.Count} entries");
            /*await using var db = await _factory.CreateDbContextAsync(cancellationToken);
            db.ChangeTracker.AutoDetectChangesEnabled = false;
            db.Logs.AddRange(batch);
            await db.SaveChangesAsync(cancellationToken);*/
        }

        private async Task FlushAsync(List<LogEntry> batch, CancellationToken cancellationToken)
        {
            for (var attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    timeout.CancelAfter(CancellationTimeout);
                    await FlushOnceAsync(batch, timeout.Token);
                    return;
                }
                catch (Exception ex) when (attempt < 3)
                {
                    Console.Error.WriteLine($"Flush attempt {attempt} failed {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2), cancellationToken);
                }
            }
        }
    }
}
