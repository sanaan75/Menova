using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Menus;
using Services.Models;
using Web.APIs;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController, ApiAuthorize]
public class MenuController(
    IDb db,
    ISearchMenus searchMenus,
    IEditMenu editMenu,
    IActorService actorService)
    : Controller
{
    [HttpPost, Route("Edit")]
    public IActionResult Edit(IEditMenu.Request request)
    {
        editMenu.Respond(request);
        db.Save();

        return Ok();
    }

    [HttpPost, Route("Menu")]
    public IActionResult Menu()
    {
        var menu = searchMenus.Respond(new ISearchMenus.Request
        {
            UserId = actorService.UserId
        }).Single();

        return Ok(new ApiResponseModel
        {
            Data = menu
        });
    }
}