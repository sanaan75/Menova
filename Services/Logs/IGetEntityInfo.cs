namespace Services.Logs;

public interface IGetEntityInfo
{
    Type Type { get; }
    
    string DefaultCaption { get; }
    
    string GetTitle(int? id);
}