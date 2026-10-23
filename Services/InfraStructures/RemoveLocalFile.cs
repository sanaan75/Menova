using Microsoft.AspNetCore.Hosting;

namespace Services.InfraStructures;

public class RemoveLocalFile(IWebHostEnvironment env) : IRemoveLocalFile
{
    public void Respond(IRemoveLocalFile.Request request)
    {
        var folderName = request.Folder.ToString().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(request.FileUrl) || !request.FileUrl.StartsWith($"/{folderName}/", StringComparison.Ordinal))
            throw new ArgumentException("The file URL does not belong to the requested folder.", nameof(request));

        var folderPath = Path.GetFullPath(Path.Combine(env.WebRootPath, folderName));
        var filePath = Path.GetFullPath(Path.Combine(env.WebRootPath,
            request.FileUrl[1..].Replace('/', Path.DirectorySeparatorChar)));
        if (!filePath.StartsWith(folderPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("The file URL is outside the requested folder.", nameof(request));

        File.Delete(filePath);
    }
}
