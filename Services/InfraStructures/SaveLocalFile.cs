using System.Globalization;
using Entities;
using Entities.Files;
using Microsoft.AspNetCore.Hosting;

namespace Services.InfraStructures;

public class SaveLocalFile(IWebHostEnvironment env, ITimeService timeService) : ISaveLocalFile
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly HashSet<string> ProgramFileExtensions = new(
        FileExtensions.Images.Concat(FileExtensions.Documents).Concat(FileExtensions.Compressions).Concat(FileExtensions.Videos).Append(".webp"),
        StringComparer.OrdinalIgnoreCase);

    public string Respond(ISaveLocalFile.Request request)
    {
        Check.False(request.File is null || request.File.Length == 0, () => MessageFactory.EmptyOrMissing(Glossary.File));

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        var allowedExtensions = request.Folder == Folders.Items ? ProgramFileExtensions : AllowedExtensions;
        Check.Contains(allowedExtensions, extension, () => MessageFactory.UnsupportedFormat(Glossary.File, allowedExtensions.Select(i => i.TrimStart('.'))));

        var folderName = request.Folder.ToString().ToLowerInvariant();
        var dateFolder = timeService.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var fileName = $"{Guid.NewGuid():N}{extension}";

        var folderPath = Path.Combine(env.WebRootPath, folderName, dateFolder);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        request.File.CopyTo(stream);

        return $"/{folderName}/{dateFolder}/{fileName}";
    }
}
