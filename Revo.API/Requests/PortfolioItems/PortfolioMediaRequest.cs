
using Revo.Domain.Enums;

namespace Revo.API.Requests.PortfolioItems
{
    public class PortfolioMediaRequest
    {
        public MediaType Type { get; set; } // 1 for Image, 2 for Video
        public int OrderIndex { get; set; }

        public IFormFile? File { get; set; } 
        public string? VideoUrl { get; set; } 
        public IFormFile? CoverImage { get; set; } 
    }
}
