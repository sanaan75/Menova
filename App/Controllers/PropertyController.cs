using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Menus.Items.Properties;
using Services.Models;
using Web.APIs;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController, ApiAuthorize]
public class PropertyController(
    IDb db,
    ISearchMenuItemProperty searchMenuItemProperty,
    IAddMenuItemProperty addMenuItemProperty)
    : Controller
{
    [HttpPost, Route("List")]
    public IActionResult List(ISearchMenuItemProperty.Request request)
    {
        var items = searchMenuItemProperty.Respond(request);

        return Ok(new ApiResponseModel
        {
            Data = items
        });
    }

    [HttpPost, Route("Add")]
    public IActionResult Add(IAddMenuItemProperty.Request request)
    {
        addMenuItemProperty.Respond(request);
        db.Save();

        return Ok();
    }
}