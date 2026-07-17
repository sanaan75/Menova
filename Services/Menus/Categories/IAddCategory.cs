using Entities.Menus;
using Microsoft.AspNetCore.Http;

namespace Services.Menus.Categories;

public interface IAddCategory
{
    Category Respond(Request request);

    class Request
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public IFormFile Image { get; set; }
    }
}