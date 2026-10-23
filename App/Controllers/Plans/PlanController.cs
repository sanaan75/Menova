using Entities;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models;
using Services.Plans;
using Web.APIs;

namespace App.Controllers.Plans;

[Route("api/[controller]")]
[ApiController, ApiAuthorize]
public class PlanController(
    IDb db,
    IActorService actorService,
    ISearchPlans searchPlans,
    IAddPlan addPlan,
    ISearchUserPlans searchUserPlans,
    IAddUserPlan addUserPlan)
    : Controller
{
    [HttpPost, Route("List")]
    public IActionResult List(ISearchPlans.Request request)
    {
        var searchRequest = IsPlatformAdmin
            ? request
            : new ISearchPlans.Request
            {
                Id = request.Id,
                IgnoredId = request.IgnoredId,
                Name = request.Name,
                IsActive = true
            };

        var items = searchPlans.Respond(searchRequest).Select(i => new
        {
            i.Id,
            i.Name,
            i.MaxItems,
            i.MaxImagePerItem,
            i.Price,
            i.IsActive
        }).ToList();

        return Ok(new ApiResponseModel
        {
            Data = items
        });
    }

    [HttpPost, Route("Add")]
    public IActionResult Add(IAddPlan.Request request)
    {
        if (!IsPlatformAdmin)
            return StatusCode(StatusCodes.Status403Forbidden);

        addPlan.Respond(request);
        db.Save();

        return Ok();
    }

    [HttpPost, Route("UserPlanList")]
    public IActionResult UserPlanList(ISearchUserPlans.Request request)
    {
        var searchRequest = IsPlatformAdmin
            ? request
            : new ISearchUserPlans.Request
            {
                Id = request.Id,
                IgnoredId = request.IgnoredId,
                UserId = actorService.UserId,
                PlanId = request.PlanId
            };

        var items = searchUserPlans.Respond(searchRequest).Select(i => new
        {
            i.Id,
            i.UserId,
            UserName = i.User.Name,
            i.PlanId,
            PlanName = i.Plan.Name,
            i.StartDate,
            i.EndDate
        }).ToList();

        return Ok(new ApiResponseModel
        {
            Data = items
        });
    }

    [HttpPost, Route("AddUserPlan")]
    public IActionResult AddUserPlan(IAddUserPlan.Request request)
    {
        if (!IsPlatformAdmin)
            return StatusCode(StatusCodes.Status403Forbidden);

        addUserPlan.Respond(request);
        db.Save();

        return Ok();
    }

    private bool IsPlatformAdmin => actorService.IsAdmin;
}
