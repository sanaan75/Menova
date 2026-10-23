namespace Services.InfraStructures;

public interface IRemoveLocalFile
{
    void Respond(Request request);

    class Request
    {
        public Folders Folder { get; set; }
        public string FileUrl { get; set; }
    }
}
