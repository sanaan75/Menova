using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Menus;
using Services.Menus.Categories;
using Services.Models;
using Web.APIs;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController, ApiAuthorize]
public class CategoryController(
    IDb db,
    IActorService actorService,
    ISearchCategories searchCategories,
    IAddCategory addCategory,
    IGetCategoryInfo getCategoryInfo,
    IEditCategory editCategory,
    IDeleteCategory deleteCategory)
    : Controller
{
    [HttpPost, Route("List")]
    public IActionResult List()
    {
        var categories = searchCategories.Respond(new ISearchCategories.Request
            {
                Menu = new ISearchMenus.Request
                {
                    UserId = actorService.UserId
                }
            })
            .Select(i => new
            {
                i.Id,
                i.MenuId,
                i.Name,
                i.Description,
                i.Order,
                i.ImageUrl,
                i.IsActive
            }).ToList();

        return Ok(new ApiResponseModel
        {
            Data = categories
        });
    }


    [HttpPost, Route("Add")]
    public IActionResult Add(IAddCategory.Request request)
    {
        addCategory.Respond(request);
        db.Save();

        return Ok();
    }

    [HttpPost, Route("Info")]
    public IActionResult Info(IdModel request)
    {
        return Ok(new ApiResponseModel
        {
            Data = getCategoryInfo.Respond(request.Id)
        });
    }

    [HttpPost, Route("Edit")]
    public IActionResult Edit(IEditCategory.Request request)
    {
        editCategory.Respond(request);
        db.Save();

        return Ok();
    }

    [HttpPost, Route("Remove")]
    public IActionResult Remove(IdModel request)
    {
        deleteCategory.Respond(request.Id);
        db.Save();

        return Ok();
    }
}