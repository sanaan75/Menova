using Entities.Plans;

namespace Services.Plans;

public interface IAddPlan
{
    Plan Respond(Request request);

    class Request
    {
        public string Name { get; set; }
        public int MaxItems { get; set; }
        public int MaxImagePerItem { get; set; }
        public int Price { get; set; }
    }
}