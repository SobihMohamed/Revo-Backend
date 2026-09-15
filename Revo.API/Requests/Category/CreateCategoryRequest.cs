namespace Revo.API.Requests.Category
{
    public record CreateCategoryRequest(string NameAr, string NameEn, int OrderIndex, IFormFile Image);
}
