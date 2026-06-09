namespace Services.Menus.Categories;

public interface IEditCategory
{
    void Respond(Request request);

    class Request
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string? ImageUrl { get; set; }
    }
}