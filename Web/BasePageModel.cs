using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web;

public class BasePageModel : PageModel
{
    public BasePageModel()
    {
    }

    [TempData] public string Info_Message { get; set; } 
    [TempData] public string Error_Message { get; set; } 
    [TempData] public string Success_Message { get; set; } 
    [TempData] public string Warning_Message { get; set; } 
}