using Entities;

namespace Services.Logs;

public abstract class BaseGetEntityInfo<T> : IGetEntityInfo
{
    public Type Type => typeof(T);
    public string DefaultCaption => GlossaryHelper.Get(Type.Name);
    
    public abstract string GetTitle(int? id);
}