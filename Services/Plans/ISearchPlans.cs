using Entities.Plans;

namespace Services.Plans;

public interface ISearchPlans
{
    IQueryable<Plan> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}