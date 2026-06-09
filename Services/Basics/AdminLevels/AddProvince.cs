using Entities;
using Entities.Basics.AdminLevels;

namespace Services.Basics.AdminLevels;

public class AddProvince(IDb db, ISearchProvinces searchProvinces, IProvinceCache provinceCache) : IAddProvince
{
    public Province Respond(string name)
    {
        var cleanedName = name.Clean();
        Check.Given(cleanedName, () => ErrorMessagePersian.Unknown(Glossary.Name));

        var isDuplicate = searchProvinces.Respond().Any(i => i.Name == cleanedName);
        Check.False(isDuplicate, () => ErrorMessagePersian.Duplicate(Glossary.Province));

        db.AddPostSaveAction(provinceCache.Reset);

        return db.Set<Province>().Add(new Province { Name = cleanedName }).Entity;
    }
}