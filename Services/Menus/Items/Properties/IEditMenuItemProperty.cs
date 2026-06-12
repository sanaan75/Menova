namespace Services.Menus.Items.Properties;

public interface IEditMenuItemProperty
{
    void Respond(Request request);

    class Request
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
    }
}