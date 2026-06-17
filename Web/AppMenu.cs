using Entities.Basics.Security;
using Services;

namespace Web;

public abstract class AppMenu : IMenu
{
    private readonly IActorService _actorService;

    public abstract int Order { get; }
    public abstract string Title { get; }
    public virtual string BadgeUrl => null;
    public abstract bool Visible { get; }

    private List<IMenu.MenuItem> _items;

    protected AppMenu(IActorService actorService)
    {
        _actorService = actorService;
    }

    public List<IMenu.MenuItem> GenerateItems()
    {
        if (_items != null)
            return _items;

        _items = new List<IMenu.MenuItem>();
        AddItems();
        return _items;
    }

    protected abstract void AddItems();

    protected IMenu.MenuItem AddItem(string title, string url)
    {
        var item = new IMenu.MenuItem
        {
            Title = title,
            Url = url,
        };
        _items.Add(item);
        return item;
    }

    protected IMenu.MenuItem AddItemIf(bool condition, string title, string url)
    {
        if (condition)
            AddItem(title, url);

        return null;
    }

    protected bool HasPermission(Permission permission)
    {
        if (_actorService.IsSystemAdmin)
            return true;

        return  _actorService.IsSystemAdmin | _actorService.HasPermission(permission);
    }

    protected bool IsSysAdmin => _actorService.IsAuthenticated;
}