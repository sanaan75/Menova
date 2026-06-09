namespace Services.Basics.AdminLevels;

public interface ICountyCache : ISingleInstance
{
    public void Reset();

    public Model Get(int? id);

    public IList<Model> GetAll();
    public IList<Model> GetProvinceCounties(int id);

    class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProvinceId { get; set; }
    }
}