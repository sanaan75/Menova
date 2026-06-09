using Entities.Basics.AdminLevels;

namespace Services.Basics.AdminLevels;

public interface IAddCounty
{
    County Respond(int provinceId, string name);
}