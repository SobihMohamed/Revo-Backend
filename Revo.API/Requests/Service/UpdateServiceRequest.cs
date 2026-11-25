using System.ComponentModel.DataAnnotations;

namespace Revo.API.Requests.Service
{
    public class UpdateServiceRequest
    {
        public string? NameAr { get; set; }

        [Required]
        public string NameEn { get; set; } = string.Empty;

        public string? DescriptionAr { get; set; }

        [Required]
        public string DescriptionEn { get; set; } = string.Empty;

        public int OrderIndex { get; set; }

        public IFormFile? Image { get; set; }
    }
}