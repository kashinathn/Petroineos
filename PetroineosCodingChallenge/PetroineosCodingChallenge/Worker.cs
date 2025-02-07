using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace PetroineosCodingChallenge
{
    public class PowerPositionWorker : BackgroundService
    {
        private readonly PowerPositionSettings _settings;
        private readonly PowerPositionCalculator _calculator;
        private readonly IFileWriter _fileWriter;
        private readonly ITimeConverter _timeConverter;
        private readonly ILogger<PowerPositionWorker> _logger;
        private DateTime _nextRunTime;

        public PowerPositionWorker(
            IOptions<PowerPositionSettings> settings,
            PowerPositionCalculator calculator,
            IFileWriter fileWriter,
            ITimeConverter timeConverter,
            ILogger<PowerPositionWorker> logger)
        {
            _settings = settings.Value;
            _calculator = calculator;
            _fileWriter = fileWriter;
            _timeConverter = timeConverter;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service starting. Initial interval: {Interval} minutes",
                _settings.IntervalMinutes);

            await RunExtractAsync();
            ScheduleNextRun();

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = CalculateDelay();
                _logger.LogDebug("Next extract scheduled in {TotalMinutes:N1} minutes",
                    delay.TotalMinutes);

                await Task.Delay(delay, stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await RunExtractAsync();
                    ScheduleNextRun();
                }
            }
        }

        private void ScheduleNextRun()
        {
            _nextRunTime = DateTime.UtcNow.AddMinutes(_settings.IntervalMinutes);
            _logger.LogInformation("Next scheduled run at {NextRunTime:yyyy-MM-dd HH:mm:ss}",
                _nextRunTime);
        }

        private TimeSpan CalculateDelay()
        {
            var now = DateTime.UtcNow;
            var timeUntilNextRun = _nextRunTime - now;
            return timeUntilNextRun > TimeSpan.Zero ? timeUntilNextRun : TimeSpan.Zero;
        }

        private async Task RunExtractAsync()
        {
            try
            {
                _logger.LogInformation("Starting extract process");
                var stopwatch = Stopwatch.StartNew();

                var csvContent = await _calculator.CalculatePositionsAsync();
                var fileName = GenerateFileName();
                var fullPath = Path.Combine(_settings.OutputPath, fileName);

                await _fileWriter.WriteFileAsync(fullPath, csvContent);

                _logger.LogInformation("Extract completed successfully in {ElapsedMs} ms. File: {FileName}",
                    stopwatch.ElapsedMilliseconds, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Fatal error during extract process");
            }
        }

        private string GenerateFileName()
        {
            var londonTime = _timeConverter.GetLondonTime();
            return $"PowerPosition_{londonTime:yyyyMMdd}_{londonTime:HHmm}.csv";
        }
    }

    
}
