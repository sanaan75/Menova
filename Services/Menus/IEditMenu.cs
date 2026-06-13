namespace Services.Menus;

public interface IEditMenu
{
    void Respond(Request request);

    class Request
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}