namespace Revo.API.Requests.Category
{
    public record UpdateCategoryRequest(
        string NameAr,
        string NameEn,
        int OrderIndex,
        IFormFile? Image
        );
}
