using Entities;
using Entities.Basics.AdminLevels;

namespace Services.Basics.AdminLevels;

public class AddCounty(IDb db, ISearchCounties searchCounties, ICountyCache countyCache) : IAddCounty
{
    public County Respond(int provinceId, string name)
    {
        var cleanedName = name.Clean();
        Check.Given(cleanedName, () => ErrorMessage.Unknown(Glossary.Name));

        var isDuplicate = searchCounties.Respond(new ISearchCounties.Request
        {
            ProvinceId = provinceId
        }).Any(i => i.Name == cleanedName);
        
        Check.False(isDuplicate, () => ErrorMessage.Duplicate(Glossary.Province));

        db.AddPostSaveAction(countyCache.Reset);

        return db.Set<County>().Add(new County { Name = cleanedName }).Entity;
    }
}