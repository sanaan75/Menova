using Entities.Menus;
using Microsoft.AspNetCore.Http;

namespace Services.Menus.ItemImages;

public interface IAddMenuItemImage
{
    MenuItemImage Respond(Request request);

    class Request
    {
        public int MenuItemId { get; set; }
        public IFormFile Image { get; set; }
    }
}