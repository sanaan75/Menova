namespace Services.Plans;

public interface IPlanCache : ISingleInstance
{
    public void Reset();

    public Model Get(int? id);

    public IList<Model> GetAll();

    class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxItems { get; set; }
        public int MaxImagePerItem { get; set; }
        public int Price { get; set; }
        public bool IsActive { get; set; }
    }
}