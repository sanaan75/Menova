using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Menus;
using Services.Models;
using Web.APIs;

namespace App.Controllers.Cafes;

[Route("api/basics/[controller]")]
[ApiController, ApiAuthorize]
public class MenuController(
    IDb db,
    ISearchMenus searchMenus,
    IActorService actorService)
    : Controller
{
    [HttpPost, Route("Add")]
    public IActionResult Add()
    {
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
            Data =  menu
        });
    }


    public class AddProvinceModel
    {
        public string Name { get; set; }
    }
}