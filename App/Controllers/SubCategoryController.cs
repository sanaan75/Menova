using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Menus;
using Services.Menus.Categories;
using Services.Menus.SubCategories;
using Services.Models;
using Web.APIs;

namespace App.Controllers;

[Route("api/[controller]")]
[ApiController, ApiAuthorize]
public class SubCategoryController(
    IDb db,
    ISearchSubCategories searchSubCategories,
    IAddSubCategory addSubCategory,
    IEditSubCategory editSubCategory,
    IDeleteSubCategory deleteSubCategory,
    IActorService actorService)
    : Controller
{
    [HttpPost, Route("List")]
    public IActionResult List(SearchSubCategoryModel request)
    {
        var subCategories = searchSubCategories.Respond(new ISearchSubCategories.Request
            {
                Id = request.Id,
                Category = new ISearchCategories.Request
                {
                    Id = request.CategoryId,
                    Menu = new ISearchMenus.Request
                    {
                        UserId = actorService.UserId
                    }
                }
            })
            .Select(i => new
            {
                i.Id,
                i.CategoryId,
                i.Name,
                i.Description,
                i.Order,
                i.ImageUrl,
                i.IsActive,
                Notifies = i.Notifies.Select(j => new
                {
                    j.Id,
                    j.Title,
                    j.Description,
                    j.IsActive
                })
            }).ToList();

        return Ok(new ApiResponseModel
        {
            Data = subCategories
        });
    }

    
    [HttpPost, Route("Add")]
    public IActionResult Add(IAddSubCategory.Request request)
    {
        addSubCategory.Respond(request);
        db.Save();

        return Ok();
    }

    
    [HttpPost, Route("Edit")]
    public IActionResult Edit(IEditSubCategory.Request request)
    {
        editSubCategory.Respond(request);
        db.Save();

        return Ok();
    }
    
    
    [HttpPost, Route("Remove")]
    public IActionResult Remove(IdModel request)
    {
        deleteSubCategory.Respond(request.Id);
        db.Save();

        return Ok();
    }

    public class SearchSubCategoryModel
    {
        public int? Id { get; set; }
        public int? CategoryId { get; set; }
    }
}