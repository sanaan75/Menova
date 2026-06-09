using Entities;
using Entities.Plans;

namespace Services.Plans;

public class AddPlan(IDb db, ISearchPlans searchPlans, IPlanCache planCache) : IAddPlan
{
    public Plan Respond(IAddPlan.Request request)
    {
        Check.Positive(request.MaxItems, () => ErrorMessagePersian.NotAllowed("حداکثر تعداد"));
        Check.Positive(request.Price, () => ErrorMessagePersian.NotAllowed(Glossary.Price));
        Check.Given(request.Name, () => ErrorMessagePersian.Unknown(Glossary.Name));

        var duplicate = searchPlans.Respond(new ISearchPlans.Request
        {
            Name = request.Name,
        }).Any();

        Check.False(duplicate, () => ErrorMessagePersian.Duplicate(Glossary.Plan));

        db.AddPostSaveAction(planCache.Reset);
        
        return db.Set<Plan>().Add(new Plan
        {
            Name = request.Name,
            MaxItems = request.MaxItems,
            MaxImagePerItem = request.MaxImagePerItem,
            Price = request.Price,
            IsActive = true
        }).Entity;
    }
}