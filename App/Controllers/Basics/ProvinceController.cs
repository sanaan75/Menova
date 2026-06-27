using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Basics.AdminLevels;
using Services.Models;
using Web.APIs;

namespace App.Controllers.Basics;

[Route("api/basics/[controller]")]
[ApiController]
public class ProvinceController(
    IDb db,
    IAddProvince addProvince,
    ISearchProvinces searchProvinces,
    IProvinceCache provinceCache)
    : Controller
{
    [HttpPost, Route("Add")]
    public IActionResult Add(AddProvinceModel request)
    {
        addProvince.Respond(request.Name);
        db.Save();

        return Ok();
    }


    [HttpPost, Route("List")]
    public IActionResult List(PaginationModel request)
    {
        var items = provinceCache.GetAll();
        var result = provinceCache.GetAll().Skip((request.Page - 1) * request.Size).Take(request.Size).ToList();

        return Ok(new ApiResponseModel
        {
            Data = new
            {
                Page = request.Page,
                Size = request.Size,
                Items = result,
                TotalItems = items.Count
            }
        });
    }


    [HttpPost, Route("Search")]
    public IActionResult Search(SearchProvinceModel request)
    {
        var items = searchProvinces.Respond(new ISearchProvinces.Request
        {
            Id = request.Id,
            Name = request.Name
        }).Select(i => new
        {
            i.Id,
            i.Name,
            Counties = i.Counties.Count
        }).ToList();

        return Ok(new { Data = items });
    }


    public class AddProvinceModel
    {
        public string Name { get; set; }
    }

    public class SearchProvinceModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}