using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Basics.AdminLevels;

namespace App.Controllers.Basics;

[Route("api/basics/[controller]")]
[ApiController]
public class CountyController(IDb db, IAddCounty addCounty, ICountyCache countyCache) : Controller
{
    [HttpPost, Route("Add")]
    public IActionResult Add(AddCountyModel request)
    {
        addCounty.Respond(request.ProvinceId, request.Name);
        db.Save();

        return Ok();
    }

    [HttpPost, Route("List")]
    public IActionResult List(ListModel request)
    {
        var items = countyCache.GetProvinceCounties(request.ProvinceId);
        return Ok(new { Data = items });
    }

    public class ListModel
    {
        public int ProvinceId { get; set; }
    }

    public class AddCountyModel
    {
        public int ProvinceId { get; set; }
        public string Name { get; set; }
    }
}