using Entities.Plans;

namespace Services.Plans;

public interface IAddUserPlan
{
    UserPlan Respond(Request request);

    class Request
    {
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}