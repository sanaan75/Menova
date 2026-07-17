using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Services.InfraStructures;

public class SaveLocalFile(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : ISaveLocalFile
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    public string Respond(ISaveLocalFile.Request request)
    {
        Check.False(request.File is null || request.File.Length == 0, () => ErrorMessagePersian.Invalid(Glossary.File));

        var extension = Path.GetExtension(request.File.FileName);
        Check.Contains(AllowedExtensions, extension, () => ErrorMessagePersian.Invalid("پسوند فایل"));

        var folderName = request.Folder.ToString();
        var fileName = $"{Guid.NewGuid():N}{extension}";

        var folderPath = Path.Combine(env.WebRootPath, "menu", folderName);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        request.File.CopyTo(stream);

        var httpContext = httpContextAccessor.HttpContext;
        var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

        return $"{baseUrl}/menu/{folderName}/{fileName}";
    }
}