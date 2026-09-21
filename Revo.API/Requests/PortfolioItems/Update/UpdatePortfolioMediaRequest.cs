using Revo.Domain.Enums;

namespace Revo.API.Requests.PortfolioItems.Update
{
    public class UpdatePortfolioMediaRequest
    {
        public Guid? Id { get; set; }
        public MediaType Type { get; set; }
        public int OrderIndex { get; set; }

        public IFormFile? File { get; set; }
        public string? VideoUrl { get; set; }
        public IFormFile? CoverImage { get; set; }
    }
}
