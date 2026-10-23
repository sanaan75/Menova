using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Logs;

namespace Web;

public static class ExceptionHandlerExtensions
{
    private const string DefaultSuccessMessage = "Operation successful";
    private const string LoggerCategory = "Web.PageOperations";

    public static IActionResult TryAndCacheHandleExceptions(
        this BasePageModel pageModel,
        Action action,
        string successMessage = DefaultSuccessMessage)
    {
        ArgumentNullException.ThrowIfNull(pageModel);
        ArgumentNullException.ThrowIfNull(action);

        return Execute(
            pageModel,
            action,
            pageModel.SecureRedirectToPage,
            successMessage,
            redirectOnError: false);
    }

    public static IActionResult TryAndCacheException(
        this BasePageModel pageModel,
        Action action,
        string successMessage = DefaultSuccessMessage)
    {
        ArgumentNullException.ThrowIfNull(pageModel);
        ArgumentNullException.ThrowIfNull(action);

        return Execute(
            pageModel,
            action,
            pageModel.SecureRedirectToPage,
            successMessage,
            redirectOnError: true);
    }

    public static IActionResult TryAndRedirect(
        this BasePageModel pageModel,
        Action action,
        Func<IActionResult> successResult,
        string successMessage = DefaultSuccessMessage)
    {
        ArgumentNullException.ThrowIfNull(pageModel);
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(successResult);

        return Execute(
            pageModel,
            action,
            successResult,
            successMessage,
            redirectOnError: true);
    }

    public static IActionResult SecureRedirectToPage(this BasePageModel pageModel)
    {
        ArgumentNullException.ThrowIfNull(pageModel);

        var routeValues = GetCurrentQueryValues(pageModel);
        var pageUrl = pageModel.Url.Page(
            pageModel.PageContext.ActionDescriptor.ViewEnginePath,
            routeValues);

        if (string.IsNullOrWhiteSpace(pageUrl))
            throw new InvalidOperationException("Unable to generate the target page URL.");

        //routeValues["sec"] = UrlSigner.Sign(pageUrl);

        return pageModel.RedirectToPage(routeValues);
    }

    private static IActionResult Execute(
        BasePageModel pageModel,
        Action action,
        Func<IActionResult> successResult,
        string successMessage,
        bool redirectOnError)
    {
        if (pageModel.ModelState.IsValid == false)
        {
            LogError(pageModel, StatusCodes.Status400BadRequest, null, GetModelStateMessage(pageModel));
            return pageModel.Page();
        }

        try
        {
            action();

            pageModel.ModelState.Clear();
            pageModel.Success_Message = successMessage;

            return successResult();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            HandleException(pageModel, exception);

            return redirectOnError
                ? pageModel.SecureRedirectToPage()
                : pageModel.Page();
        }
    }

    private static RouteValueDictionary GetCurrentQueryValues(BasePageModel pageModel)
    {
        var routeValues = new RouteValueDictionary();

        foreach (var queryValue in pageModel.Request.Query)
        {
            if (queryValue.Key.Equals("sec", StringComparison.OrdinalIgnoreCase) ||
                queryValue.Key.Equals("handler", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            routeValues[queryValue.Key] = queryValue.Value.Count == 1
                ? queryValue.Value[0]
                : queryValue.Value.ToArray();
        }

        return routeValues;
    }

    private static void HandleException(
        BasePageModel pageModel,
        Exception exception)
    {
        var logger = pageModel.HttpContext.RequestServices
            .GetService<ILoggerFactory>()
            ?.CreateLogger(LoggerCategory);

        if (exception is AppException appException)
        {
            LogError(pageModel, StatusCodes.Status400BadRequest, exception, appException.Message);
            pageModel.Error_Message = string.IsNullOrWhiteSpace(appException.Message)
                ? DefaultMessages.AppError()
                : appException.Message;

            logger?.LogInformation(
                "Handled application error for page {Page}. TraceId: {TraceId}",
                GetPageName(pageModel),
                pageModel.HttpContext.TraceIdentifier);

            return;
        }

        LogError(pageModel, StatusCodes.Status500InternalServerError, exception, DefaultMessages.AppError());
        pageModel.Error_Message = DefaultMessages.AppError();

        logger?.LogError(
            exception,
            "Unhandled exception for page {Page}. TraceId: {TraceId}",
            GetPageName(pageModel),
            pageModel.HttpContext.TraceIdentifier);
    }

    private static string GetPageName(BasePageModel pageModel)
    {
        return pageModel.PageContext.ActionDescriptor.ViewEnginePath;
    }

    private static void LogError(BasePageModel pageModel, int statusCode, Exception? exception, string? message)
    {
        var addErrorLog = pageModel.HttpContext.RequestServices.GetService<IAddErrorLog>();

        addErrorLog?.Respond(new IAddErrorLog.Request
        {
            StatusCode = statusCode,
            Path = pageModel.HttpContext.Request.Path.ToString(),
            IpAddress = pageModel.HttpContext.Connection.RemoteIpAddress,
            Message = message,
            Exception = exception
        });
        ErrorLogMiddleware.MarkAsLogged(pageModel.HttpContext);
    }

    private static string GetModelStateMessage(BasePageModel pageModel)
    {
        return string.Join(" ", pageModel.ModelState.Values
            .SelectMany(item => item.Errors)
            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? error.Exception?.Message : error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message)));
    }
}
