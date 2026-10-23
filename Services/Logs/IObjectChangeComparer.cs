namespace Services.Logs;

public interface IObjectChangeComparer
{
    public void Respond(object original, object updated, string section);
    public void Respond(object original, string section);

    public string GetChangesTextAndReset();
    public string GetChangesTextWithOutAndReset();
}