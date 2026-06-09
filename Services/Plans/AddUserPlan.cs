using Entities;
using Entities.Plans;

namespace Services.Plans;

public class AddUserPlan(IDb db, ISearchUserPlans searchUserPlans) : IAddUserPlan
{
    public UserPlan Respond(IAddUserPlan.Request request)
    {
        Check.Positive(request.UserId, () => ErrorMessagePersian.Unknown(Glossary.User));
        Check.Positive(request.PlanId, () => ErrorMessagePersian.Unknown(Glossary.Plan));
        Check.NotNull(request.StartDate, () => ErrorMessagePersian.Unknown(Glossary.StartDate));

        var duplicate = searchUserPlans.Respond(new ISearchUserPlans.Request
        {
            UserId = request.UserId,
            PlanId = request.PlanId,
        }).Any();

        Check.False(duplicate, () => ErrorMessagePersian.Duplicate("طرح برای کاربر"));

        return db.Set<UserPlan>().Add(new UserPlan
        {
            UserId = request.UserId,
            PlanId = request.PlanId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
        }).Entity;
    }
}