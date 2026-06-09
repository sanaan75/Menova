using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Basics.AdminLevels;
using Web.APIs;

namespace App.Controllers.Basics;

[Route("api/basics/[controller]")]
[ApiController,ApiAuthorize]
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
    public IActionResult List()
    {
        var items = provinceCache.GetAll();

        return Ok(items);
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