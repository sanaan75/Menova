using Entities.Basics.AdminLevels;

namespace Services.Basics.AdminLevels;

public interface ISearchCounties
{
    IQueryable<County> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? ProvinceId { get; set; }
        public string Name { get; set; }
    }
}