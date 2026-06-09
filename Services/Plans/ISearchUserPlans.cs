using Entities.Plans;

namespace Services.Plans;

public interface ISearchUserPlans
{
    IQueryable<UserPlan> Respond(Request request = null);

    class Request
    {
        public int? Id { get; set; }
        public int? IgnoredId { get; set; }
        public int? UserId { get; set; }
        public int? PlanId { get; set; }
    }
}