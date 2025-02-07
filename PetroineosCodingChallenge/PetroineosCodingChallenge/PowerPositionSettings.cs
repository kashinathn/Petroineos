using System.ComponentModel.DataAnnotations;

namespace PetroineosCodingChallenge
{
    public class PowerPositionSettings
    {
        public const string SectionName = "PowerPositionSettings";

        [Required]
        public string OutputPath { get; set; }

        [Range(1, 60)]
        public int IntervalMinutes { get; set; }
    }
}
