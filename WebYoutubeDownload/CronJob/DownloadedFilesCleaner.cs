using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebYoutubeDownload.Service;

namespace WebYoutubeDownload.CronJob
{
    public class DownloadedFilesCleaner : BackgroundService
    {
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun = DateTime.Now;
        private readonly ILogger<DownloadedFilesCleaner> _logger;
        private readonly DownloadedCache _downloadedCache;

        /// <summary>
        /// * * * * * *
        ///- - - - - -
        ///| | | | | |
        ///| | | | | +--- day of week(0 - 6) (Sunday=0)
        ///| | | | +----- month(1 - 12)
        ///| | | +------- day of month(1 - 31)
        ///| | +--------- hour(0 - 23)
        ///| +----------- min(0 - 59)
        ///+------------- sec(0 - 59)
        /// </summary>
        private string Schedule => "0 0 */2 * * *"; //Runs every 2 hours
        

        public DownloadedFilesCleaner(ILogger<DownloadedFilesCleaner> logger, 
            DownloadedCache downloadedCache)
        {
            _schedule = CrontabSchedule.Parse(Schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            _logger = logger;
            _downloadedCache = downloadedCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                if (now > _nextRun)
                {
                    Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(5000, stoppingToken); //5 seconds delay
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private void Process()
        {
            _logger.LogInformation("downloaded cleaner " + DateTime.Now.ToString("F"));
            var data = _downloadedCache.GetAndReset();
            var oldFiles = data
            .Select(f => new FileInfo(f))
            .ToList();

            oldFiles.ForEach(f => {
                _logger.LogInformation($"Delete old file {f.FullName}");
                f.Delete(); });
        }
    }
}
