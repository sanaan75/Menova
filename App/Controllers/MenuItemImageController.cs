using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Menus.ItemImages;
using Services.Menus.Items;
using Services.Models;
using Web.APIs;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController, ApiAuthorize]
public class MenuItemImageController(
    IDb db,
    ISearchMenuItemImages searchMenuItemImages,
    IAddMenuItemImage addMenuItemImage,
    IDeleteMenuItemImage deleteMenuItemImage,
    IActorService actorService)
    : Controller
{
    [HttpPost, Route("List")]
    public IActionResult List(IdModel request)
    {
        var images = searchMenuItemImages.Respond(new ISearchMenuItemImages.Request
            {
                MenuItemId = request.Id,
                MenuItem = new ISearchMenuItems.Request
                {
                    UserId = actorService.UserId
                }
            })
            .Select(i => new
            {
                i.Id,
                i.Url
            }).ToList();

        return Ok(new ApiResponseModel
        {
            Data = images
        });
    }

    [HttpPost, Route("Add")]
    public IActionResult Add(IAddMenuItemImage.Request request)
    {
        addMenuItemImage.Respond(request);
        db.Save();

        return Ok();
    }

    [HttpPost, Route("Delete")]
    public IActionResult Delete(IdModel request)
    {
        deleteMenuItemImage.Respond(request.Id);
        db.Save();

        return Ok();
    }
}