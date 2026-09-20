using Revo.API.Requests.PortfolioItems.Create;

namespace Revo.API.Requests.PortfolioItems.Update
{
    public class UpdatePortfolioItemRequest
    {
        public string CaptionAr { get; set; } = string.Empty;
        public string CaptionEn { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public Guid CategoryId { get; set; }

        public List<UpdatePortfolioMediaRequest> MediaItems { get; set; } = new();
    }
}
