namespace Revo.API.Requests.PortfolioItems
{
    public class CreatePortfolioItemRequest
    {
        public string CaptionAr { get; set; } = string.Empty;
        public string CaptionEn { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public Guid CategoryId { get; set; }

        public List<PortfolioMediaRequest> MediaItems { get; set; } = new();
    }
}
