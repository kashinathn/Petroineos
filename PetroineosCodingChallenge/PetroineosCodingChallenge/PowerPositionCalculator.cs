using Services;
using System.Globalization;
using System.Text;

namespace PetroineosCodingChallenge
{
    public class PowerPositionCalculator
    {
        private readonly IPowerService _powerService;
        private readonly ITimeConverter _timeConverter;
        private readonly ILogger<PowerPositionCalculator> _logger;

        public PowerPositionCalculator(
            IPowerService powerService,
            ITimeConverter timeConverter,
            ILogger<PowerPositionCalculator> logger)
        {
            _powerService = powerService;
            _timeConverter = timeConverter;
            _logger = logger;
        }

        public async Task<StringBuilder> CalculatePositionsAsync()
        {
            try
            {
                _logger.LogInformation("Starting position calculation");

                var londonTime = _timeConverter.GetLondonTime();
                var tradeDate = _timeConverter.GetTradeDate(londonTime);

                _logger.LogDebug("Using trade date: {TradeDate} for London time: {LondonTime}",
                    tradeDate.ToString("yyyy-MM-dd"), londonTime);

                var trades = await _powerService.GetTradesAsync(tradeDate);
                _logger.LogInformation("Retrieved {Count} trades", trades.Count());

                var aggregated = AggregateTrades(trades);
                return GenerateCsvContent(aggregated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating positions");
                throw;
            }
        }

        private IEnumerable<Position> AggregateTrades(IEnumerable<PowerTrade> trades)
        {
            return trades
                .SelectMany(t => t.Periods)
                .GroupBy(p => p.Period)
                .Select(g => new Position(
                    Period: g.Key,
                    TotalVolume: g.Sum(p => p.Volume)
                ))
                .OrderBy(p => p.Period);
        }

        private StringBuilder GenerateCsvContent(IEnumerable<Position> positions)
        {
            var csvContent = new StringBuilder();
            csvContent.AppendLine("Local Time,Volume");

            _logger.LogDebug("Generating CSV content for {Count} periods", positions.Count());

            foreach (var position in positions)
            {
                var localTime = _timeConverter.GetLocalTimeFromPeriod(position.Period);
                csvContent.AppendLine($"{localTime},{position.TotalVolume.ToString(CultureInfo.InvariantCulture)}");
                _logger.LogTrace("Added position: {LocalTime} - {Volume}",
                    localTime, position.TotalVolume);
            }

            return csvContent;
        }
    }

    public record Position(int Period, double TotalVolume);
}
