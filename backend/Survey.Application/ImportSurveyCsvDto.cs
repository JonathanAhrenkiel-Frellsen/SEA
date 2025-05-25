using Microsoft.AspNetCore.Http;

namespace Survey.Application
{
    public class ImportSurveyCsvDto
    {
        public IFormFile File { get; set; }
    }
}