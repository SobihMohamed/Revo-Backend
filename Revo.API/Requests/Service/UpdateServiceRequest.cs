using System.ComponentModel.DataAnnotations;

namespace Revo.API.Requests.Service
{
    public class UpdateServiceRequest
    {
        [Required]
        public string NameAr { get; set; } = string.Empty;

        [Required]
        public string NameEn { get; set; } = string.Empty;

        [Required]
        public string DescriptionAr { get; set; } = string.Empty;

        [Required]
        public string DescriptionEn { get; set; } = string.Empty;

        public int OrderIndex { get; set; }

        public IFormFile? Image { get; set; }
    }
}
