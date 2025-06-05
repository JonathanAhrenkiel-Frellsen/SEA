using Microsoft.AspNetCore.Http;

namespace Survey.Application
{
    public class ImportSurveyCsvDto
    {
        public required IFormFile File { get; set; }
    }
}