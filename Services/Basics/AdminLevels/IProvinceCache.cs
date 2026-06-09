namespace Services.Basics.AdminLevels;

public interface IProvinceCache : ISingleInstance
{
    public void Reset();

    public Model Get(int? id);

    public IList<Model> GetAll();

    class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}