namespace Services.Plans;

public interface IUserPlanCache : ISingleInstance
{
    void Reset();

    Model Get(int? id);

    IList<Model> GetAll();

    IList<Model> GetByUser(int userId);

    class Model
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}