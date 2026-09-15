using Revo.Application.Dto;

namespace Revo.API.Extention
{
    public static class FormFileExtensions
    {
        public static ImageUploadDto? ToUploadDto(this IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            return new ImageUploadDto(file.OpenReadStream(), file.FileName);
        }
    }
}
