using Entities.Basics.AdminLevels;

namespace Services.Basics.AdminLevels;

public interface ISearchProvinces
{
    IQueryable<Province> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}