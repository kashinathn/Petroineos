namespace PetroineosCodingChallenge
{
    public interface ITimeConverter
    {
        DateTime GetLondonTime();
        DateTime GetTradeDate(DateTime londonTime);
        string GetLocalTimeFromPeriod(int period);
    }

    public class LondonTimeConverter : ITimeConverter
    {
        private readonly TimeZoneInfo _londonTimeZone;

        public LondonTimeConverter()
        {
            _londonTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        }

        public DateTime GetLondonTime() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _londonTimeZone);

        public DateTime GetTradeDate(DateTime londonTime) =>
            londonTime.Hour >= 23 ? londonTime.Date : londonTime.Date.AddDays(-1);

        public string GetLocalTimeFromPeriod(int period)
        {
            var hour = (period - 1 + 23) % 24;
            return $"{hour:D2}:00";
        }
    }
}
