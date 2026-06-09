namespace Services.Basics.Security.UserGroups;

public interface IUserGroupCache : ISingleInstance
{
    public void Reset();

    public Model Get(int? id);

    public IList<Model> GetAll();

    class Model
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}