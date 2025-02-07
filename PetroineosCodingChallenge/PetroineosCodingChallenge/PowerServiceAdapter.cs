using Services;

namespace PetroineosCodingChallenge
{
    public interface IPowerService
    {
        Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date);
    }

    // Adapter for the provided PowerService.dll
    public class PowerServiceAdapter : IPowerService, IDisposable
    {
        private readonly PowerService _powerService;
        private readonly ILogger<PowerServiceAdapter> _logger;

        public PowerServiceAdapter(ILogger<PowerServiceAdapter> logger)
        {
            _logger = logger;
            _powerService = new PowerService();
            _logger.LogDebug("PowerService adapter initialized");
        }

        public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            _logger.LogInformation("Fetching trades for {Date:yyyy-MM-dd}", date);
            try
            {
                var trades = await _powerService.GetTradesAsync(date);
                _logger.LogDebug("Retrieved {Count} trades", trades.Count());
                return trades;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve trades for {Date:yyyy-MM-dd}", date);
                throw;
            }
        }

        public void Dispose()
        {
            // dispose _powerService
            _logger.LogDebug("PowerService adapter disposed");
        }
    }
}
