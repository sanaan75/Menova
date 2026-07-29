using Entities.Menus;
using Microsoft.AspNetCore.Http;

namespace Services.Menus.SubCategories;

public interface IAddSubCategory
{
    SubCategory Respond(Request request);

    class Request
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public IFormFile Image { get; set; }
    }
}