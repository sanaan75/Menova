using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Menus.Items;
using Services.Menus.Items.Properties;
using Services.Models;
using Web.APIs;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController, ApiAuthorize]
public class PropertyController(
    IDb db,
    ISearchMenuItems searchMenuItems,
    IActorService actorService,
    ISearchMenuItemProperty searchMenuItemProperty,
    IAddMenuItemProperty addMenuItemProperty)
    : Controller
{
    [HttpPost, Route("List")]
    public IActionResult List(ISearchMenuItemProperty.Request request)
    {
        var isItemOwner = searchMenuItems.Respond(new ISearchMenuItems.Request
        {
            Id = request.MenuItemId,
            UserId = actorService.UserId
        }).Any();
        Check.True(isItemOwner, () => ErrorMessagePersian.NotAllowed($"{Glossary.Search} {Glossary.Property}"));
        
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