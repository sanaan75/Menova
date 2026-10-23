using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models;
using Services.Settings;
using Web.APIs;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController, ApiAuthorize]
public class SettingsController(
    IDb db,
    IActorService actorService,
    ISettingsCache settingsCache,
    IEditSettings editSettings,
    ISetActionLogSettings setActionLogSettings)
    : Controller
{
    [HttpPost, Route("List")]
    public IActionResult List()
    {
        if (!actorService.IsAdmin)
            return StatusCode(StatusCodes.Status403Forbidden);

        return Ok(new ApiResponseModel
        {
            Data = settingsCache.Get()
        });
    }

    [HttpPost, Route("Edit")]
    public IActionResult Edit(IEditSettings.Request request)
    {
        if (!actorService.IsAdmin)
            return StatusCode(StatusCodes.Status403Forbidden);

        editSettings.Respond(request);
        db.Save();

        return Ok();
    }

    [HttpPost, Route("SetActionLog")]
    public IActionResult SetActionLog(ISetActionLogSettings.Request request)
    {
        if (!actorService.IsAdmin)
            return StatusCode(StatusCodes.Status403Forbidden);

        setActionLogSettings.Respond(request);
        db.Save();

        return Ok();
    }
}
