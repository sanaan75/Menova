using System.Text.RegularExpressions;
using Entities;
using Services.Settings;

namespace Services.Users.Auth;

public class ValidatePasswordPattern(ISettingsCache settingsCache) : IValidatePasswordPattern
{
    public void Respond(string password)
    {
        var settings = settingsCache.Get();

        Check.AscendsOrEqual(settings.MinLengthOfPassword, password.Length, () => $"رمز عبور حداقل باید دارای {settings.MinLengthOfPassword} کارکتر باشد.");
        Check.AscendsOrEqual(password.Length, settings.MaxLengthOfPassword, () => $" رمز عبور حداکثر باید دارای {settings.MaxLengthOfPassword} کارکتر باشد. ");
        
        if (settings.PasswordContainCapitalLetters == true)
            Check.True(Regex.IsMatch(password, @"[A-Z]"), () => "رمز عبور باید حداقل یک حرف بزرگ داشته باشد.");

        if (settings.PasswordContainNumbers == true)
            Check.True(Regex.IsMatch(password, @"\d"), () => "رمز عبور باید حداقل دارای یک رقم باشد.");

        if (settings.PasswordContainSpecialLetters == true)
            Check.True(Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"), () => "رمز عبور باید حداقل یک کاراکتر خاص داشته باشد.");
    }
}