using Entities.Validations;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Apis;
using Services.Models;
using Services.Users;

namespace App.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IDb db,
    ILoginApi loginApi,
    IApiTokenService apiTokenService,
    ILogoutUser logoutUser,
    IActorService actorService)
    : Controller
{
    [Route("Login")]
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel request)
    {
        var actor = await loginApi.Respond(request.Username, request.Password);
        var (key, expire) = await apiTokenService.CreateTokenAsync(actor.UserId);

        await db.SaveAsync();

        return Ok(new ApiResponseModel
        {
            Message = "ورود موفق",
            Data = new
            {
                Token = key,
                Expire = expire,
                UserId = actor.UserId,
                Title = actor.Title,
                IsAuthenticated = actor.IsAuthenticated
            }
        });
    }

    [Route("ResetPassword")]
    [HttpPost]
    public async Task<IActionResult> ResetPassword()
    {
        // _resetPassword
        await db.SaveAsync();
        return Ok(new { Message = "ثبت نام انجام شد لطفا کد ارسال شده را جهت تایید شماره موبال ارسال نمایید" });
    }


    [Route("ResetPasswordConfirmation")]
    [HttpPost]
    public async Task<IActionResult> ResetPasswordConfirmation()
    {
        // _resetPasswordConfirmation.Respond(request.Mobile, request.ConfirmCode);
        await db.SaveAsync();
        return Ok(new { Message = "ثبت نام انجام شد لطفا کد ارسال شده را جهت تایید شماره موبال ارسال نمایید" });
    }

    [Route("Logout")]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        logoutUser.Respond(actorService.Get().Username);
        await db.SaveAsync();

        return Ok();
    }

    public class LoginModel
    {
        [AppRequired] public string Username { get; set; }
        [AppRequired] public string Password { get; set; }
    }
}