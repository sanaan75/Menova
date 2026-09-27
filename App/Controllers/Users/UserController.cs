using Entities;
using Entities.Logs;
using Entities.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Apis;
using Services.Models;
using Services.Users;
using Web.APIs;

namespace App.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IDb db,
    ILoginApi loginApi,
    IApiTokenService apiTokenService,
    IAddUser addUser,
    IAddUserLogin addUserLogin,
    ILogoutApi logoutApi,
    IActorService actorService)
    : Controller
{
    [Route("Login")]
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Login(LoginModel request)
    {
        var actor = await loginApi.Respond(request.Username, request.Password);
        var (key, expire) = apiTokenService.CreateTokenAsync(actor.UserId);
        addUserLogin.Respond(new IAddUserLogin.Request
        {
            Username = actor.Username,
            Method = UserLoginMethod.Password,
        });

        db.Save();

        return Ok(new ApiResponseModel
        {
            Message = "ورود موفق",
            Data = new
            {
                Token = key,
                Expire = expire,
                UserId = actor.UserId,
                Type = actor.Type,
                TypeCaption = actor.Type.GetCaption(),
                IsAuthenticated = actor.IsAuthenticated,
                IsSuperAdmin = actor.IsSuperAdmin
            }
        });
    }


    [Route("Logout")]
    [HttpPost, ApiAuthorize]
    public IActionResult Logout()
    {
        logoutApi.Respond(actorService.UserId);
        db.Save();

        return Ok(new ApiResponseModel
        {
            Message = "خروج"
        });
    }


    [Route("ResetPassword")]
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> ResetPassword()
    {
        // _resetPassword
        await db.SaveAsync();
        return Ok(new { Message = "ثبت نام انجام شد لطفا کد ارسال شده را جهت تایید شماره موبال ارسال نمایید" });
    }


    [Route("ResetPasswordConfirmation")]
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> ResetPasswordConfirmation()
    {
        // _resetPasswordConfirmation.Respond(request.Mobile, request.ConfirmCode);
        await db.SaveAsync();
        return Ok(new { Message = "ثبت نام انجام شد لطفا کد ارسال شده را جهت تایید شماره موبال ارسال نمایید" });
    }

    [Route("Add")]
    [HttpPost, ApiAuthorize]
    public IActionResult Add(IAddUser.Request request)
    {
        addUser.Respond(request);
        db.Save();

        return Ok();
    }

    public class LoginModel
    {
        [AppRequired] public string Username { get; set; }
        [AppRequired] public string Password { get; set; }
    }
}