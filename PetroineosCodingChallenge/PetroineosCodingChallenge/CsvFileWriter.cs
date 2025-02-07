using System.Text;

namespace PetroineosCodingChallenge
{
    public interface IFileWriter
    {
        Task WriteFileAsync(string fullPath, StringBuilder content);
    }

    public class CsvFileWriter : IFileWriter
    {
        private readonly ILogger<CsvFileWriter> _logger;

        public CsvFileWriter(ILogger<CsvFileWriter> logger)
        {
            _logger = logger;
        }

        public async Task WriteFileAsync(string fullPath, StringBuilder content)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                await File.WriteAllTextAsync(fullPath, content.ToString());
                _logger.LogDebug("Successfully wrote file: {FilePath}", fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write file: {FilePath}", fullPath);
                throw;
            }
        }
    }
}
