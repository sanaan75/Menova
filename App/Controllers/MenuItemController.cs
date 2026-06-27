using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Menus.Items;
using Services.Models;
using Web.APIs;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController, ApiAuthorize]
public class MenuItemController(
    IDb db,
    ISearchMenuItems searchMenuItems,
    IAddMenuItem addMenuItem,
    IEditMenuItem editMenuItem,
    IActorService actorService)
    : Controller
{
    [HttpPost, Route("List")]
    public IActionResult List()
    {
        var items = searchMenuItems.Respond(new ISearchMenuItems.Request
            {
                UserId = actorService.UserId
            })
            .Select(i => new
            {
                i.Id,
                i.Name,
                i.Description,
                i.Order,
                i.IsActive,
                i.SubCategoryId,
                SubCategory = i.SubCategory.Name
            }).ToList();

        return Ok(new ApiResponseModel
        {
            Data = items
        });
    }


    [HttpPost, Route("Add")]
    public IActionResult Add(IAddMenuItem.Request request)
    {
        addMenuItem.Respond(request);
        db.Save();

        return Ok();
    }


    [HttpPost, Route("Edit")]
    public IActionResult Edit(IEditMenuItem.Request request)
    {
        editMenuItem.Respond(request);
        db.Save();

        return Ok();
    }
}