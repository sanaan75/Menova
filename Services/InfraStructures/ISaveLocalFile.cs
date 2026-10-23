using Microsoft.AspNetCore.Http;

namespace Services.InfraStructures;

public interface ISaveLocalFile
{
    string Respond(Request request);

    class Request
    {
        public Folders Folder { get; set; }
        public IFormFile File { get; set; }
    }
}