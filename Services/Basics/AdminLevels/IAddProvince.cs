using Entities.Basics.AdminLevels;

namespace Services.Basics.AdminLevels;

public interface IAddProvince
{
    Province Respond(string name);
}