using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Entities;
using Entities.Attributes;

namespace Services.Logs;

public class ObjectChangeComparer(IEnumerable<IGetEntityInfo> entityInfos) : IObjectChangeComparer
{
    private static readonly Regex HtmlTagRegex = new("<[^>]*>", RegexOptions.Compiled);

    private readonly List<string> _changes = [];
    private readonly IReadOnlyList<IGetEntityInfo> _entityInfos = entityInfos?.ToList() ?? [];

    public void Respond(object original, object updated, string section)
    {
        if (original is null || updated is null)
            return;

        var originalProperties = GetComparableProperties(original.GetType());
        var updatedProperties = updated.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(IsReadableProperty)
            .ToDictionary(property => property.Name, StringComparer.Ordinal);

        var changesBefore = _changes.Count;

        foreach (var originalProperty in originalProperties)
        {
            if (!updatedProperties.TryGetValue(originalProperty.Name, out var updatedProperty))
                continue;

            // A request DTO can expose a property with a different type. In
            // that case the values are not meaningful to compare or format.
            if (!AreCompatibleTypes(originalProperty.PropertyType, updatedProperty.PropertyType))
                continue;

            var oldValue = originalProperty.GetValue(original);
            var newValue = updatedProperty.GetValue(updated);
            var formattedChange = CompareAndFormat(originalProperty, oldValue, newValue);

            if (formattedChange is null)
                continue;

            var (oldText, newText) = formattedChange.Value;
            AddChange($"{GetDisplayName(originalProperty)} از {oldText.WithPersianQuotes()} به {newText.WithPersianQuotes()} تغییر کرد");
        }

        AddSectionIfNeeded(section, changesBefore);
    }

    public void Respond(object original, string section)
    {
        if (original is null)
            return;

        var changesBefore = _changes.Count;

        foreach (var property in GetComparableProperties(original.GetType()))
        {
            var value = property.GetValue(original);

            if (value is null)
                continue;

            var text = FormatValue(property, value);

            if (text is null)
                continue;

            AddChange($"{GetDisplayName(property)}: {text}");
        }

        AddSectionIfNeeded(section, changesBefore);
    }

    public string GetChangesTextAndReset()
    {
        Check.True(_changes.Count > 0, () => MessageFactory.NoChanges());
        return GetChangesAndReset();
    }

    public string GetChangesTextWithOutAndReset()
    {
        return _changes.Count == 0 ? string.Empty : GetChangesAndReset();
    }

    private string GetChangesAndReset()
    {
        try
        {
            return string.Join("\n", _changes);
        }
        finally
        {
            _changes.Clear();
        }
    }

    private static IEnumerable<PropertyInfo> GetComparableProperties(Type type)
    {
        return type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(IsReadableProperty)
            .Where(property => !IsIgnoredProperty(property));
    }

    private static bool IsReadableProperty(PropertyInfo property)
    {
        return property.CanRead && property.GetMethod is not null && property.GetIndexParameters().Length == 0;
    }

    private static bool IsIgnoredProperty(PropertyInfo property)
    {
        if (property.Name.Equals(nameof(IEntity.Id), StringComparison.Ordinal)
            || property.IsDefined(typeof(NotMappedAttribute)))
        {
            return true;
        }

        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (typeof(IEntity).IsAssignableFrom(propertyType))
            return true;

        if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
            return true;

        return propertyType == typeof(File)
               || propertyType.FullName == "Microsoft.AspNetCore.Http.IFormFile"
               || propertyType.GetInterfaces().Any(i => i.FullName == "Microsoft.AspNetCore.Http.IFormFile");
    }

    private static bool AreCompatibleTypes(Type originalType, Type updatedType)
    {
        var originalUnderlying = Nullable.GetUnderlyingType(originalType) ?? originalType;
        var updatedUnderlying = Nullable.GetUnderlyingType(updatedType) ?? updatedType;

        return originalUnderlying == updatedUnderlying
               || originalUnderlying.IsAssignableFrom(updatedUnderlying)
               || updatedUnderlying.IsAssignableFrom(originalUnderlying);
    }

    private void AddSectionIfNeeded(string section, int changesBefore)
    {
        if (_changes.Count == changesBefore || string.IsNullOrWhiteSpace(section))
            return;

        _changes.Insert(changesBefore, section);
    }

    private void AddChange(string change)
    {
        if (!string.IsNullOrWhiteSpace(change))
            _changes.Add(change);
    }

    private string GetDisplayName(PropertyInfo property)
    {
        var glossaryName = GetGlossaryName(property.Name);

        if (!property.Name.EndsWith("Id", StringComparison.Ordinal)
            || property.Name.Equals(nameof(IEntity.Id), StringComparison.Ordinal))
        {
            return glossaryName;
        }

        var navigationName = property.Name[..^2];
        var navigationProperty = property.DeclaringType?.GetProperty(navigationName);

        if (navigationProperty is null)
            return glossaryName;

        var navigationType = Nullable.GetUnderlyingType(navigationProperty.PropertyType) ?? navigationProperty.PropertyType;
        var entityInfo = _entityInfos.FirstOrDefault(info => info.Type == navigationType);

        if (entityInfo is not null && !string.IsNullOrWhiteSpace(entityInfo.DefaultCaption))
            return entityInfo.DefaultCaption;
        
        var combinedName = navigationName + navigationType.Name;
        return TryGetGlossary(combinedName, out var combinedCaption)
            ? combinedCaption
            : TryGetGlossary(navigationType.Name, out var typeCaption)
                ? typeCaption
                : glossaryName;
    }

    private static string GetGlossaryName(string propertyName)
    {
        if (TryGetGlossary(propertyName, out var caption))
            return caption;

        if (propertyName.StartsWith("Is", StringComparison.Ordinal)
            && propertyName.Length > 2
            && TryGetGlossary(propertyName[2..], out caption))
        {
            return caption;
        }

        if (propertyName.EndsWith("Id", StringComparison.Ordinal)
            && propertyName.Length > 2
            && TryGetGlossary(propertyName[..^2], out caption))
        {
            return caption;
        }

        return GlossaryHelper.Get(propertyName);
    }

    private static bool TryGetGlossary(string key, out string caption)
    {
        if (GlossaryHelper.GetAll().TryGetValue(key, out var value)
            && !string.IsNullOrWhiteSpace(value))
        {
            caption = value;
            return true;
        }

        caption = string.Empty;
        return false;
    }

    private (string OldText, string NewText)? CompareAndFormat(PropertyInfo property, object? oldValue, object? newValue)
    {
        if (ValuesEqual(property, oldValue, newValue))
            return null;

        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (propertyType == typeof(DateTime)
            && oldValue is DateTime oldDate
            && newValue is DateTime newDate)
        {
            var dateOldText = oldDate.Date == newDate.Date
                ? oldDate.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture)
                : oldDate.ToShortDateString();
            var dateNewText = oldDate.Date == newDate.Date
                ? newDate.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture)
                : newDate.ToShortDateString();

            return dateOldText == dateNewText ? null : (dateOldText, dateNewText);
        }

        if (propertyType == typeof(DateTimeOffset)
            && oldValue is DateTimeOffset oldDateOffset
            && newValue is DateTimeOffset newDateOffset)
        {
            var oldLocal = oldDateOffset.ToLocalTime();
            var newLocal = newDateOffset.ToLocalTime();
            var dateOldText = oldLocal.Date == newLocal.Date
                ? oldLocal.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture)
                : oldLocal.ToString("d", CultureInfo.CurrentCulture);
            var dateNewText = oldLocal.Date == newLocal.Date
                ? newLocal.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture)
                : newLocal.ToString("d", CultureInfo.CurrentCulture);

            return dateOldText == dateNewText ? null : (dateOldText, dateNewText);
        }

        var oldText = FormatValue(property, oldValue);
        var newText = FormatValue(property, newValue);

        if (oldText is null || newText is null || oldText == newText)
            return null;

        return (oldText, newText);
    }

    private static bool ValuesEqual(PropertyInfo property, object? oldValue, object? newValue)
    {
        if (property.IsDefined(typeof(HtmlCompareAttribute)))
        {
            var oldHtml = StripHtml(oldValue as string);
            var newHtml = StripHtml(newValue as string);
            return string.Equals(oldHtml, newHtml, StringComparison.Ordinal);
        }

        return Equals(oldValue, newValue);
    }

    private string? FormatValue(PropertyInfo property, object? value)
    {
        if (value is null)
            return string.Empty;

        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        if (propertyType == typeof(object) || propertyType.IsInterface)
            propertyType = value.GetType();

        if (property.IsDefined(typeof(HtmlCompareAttribute)))
            return StripHtml(value as string);

        if (property.IsDefined(typeof(CurrencyCompareAttribute))
            && TryConvertDecimal(value, out var currency))
        {
            return currency?.FormatAsCurrency(true) ?? string.Empty;
        }

        if (property.IsDefined(typeof(PercentCompareAttribute))
            && TryConvertDecimal(value, out var percent))
        {
            return percent?.FormatAsPercent() ?? string.Empty;
        }

        if (propertyType == typeof(bool))
            return Convert.ToBoolean(value, CultureInfo.InvariantCulture) ? "بله" : "خیر";

        if (propertyType == typeof(DateTime))
            return ((DateTime)value).ToShortDateString();

        if (propertyType == typeof(DateTimeOffset))
            return ((DateTimeOffset)value).ToLocalTime().ToString("d", CultureInfo.CurrentCulture);

        if (propertyType.IsEnum)
            return ((Enum)value).GetCaption();

        if (propertyType == typeof(decimal) && TryConvertDecimal(value, out var decimalValue))
            return decimalValue?.NiceFormat() ?? string.Empty;

        if (propertyType == typeof(int) || propertyType == typeof(long)
            || propertyType == typeof(short) || propertyType == typeof(byte)
            || propertyType == typeof(uint) || propertyType == typeof(ulong)
            || propertyType == typeof(ushort) || propertyType == typeof(sbyte))
        {
            if (property.Name.EndsWith("Id", StringComparison.Ordinal)
                && TryGetEntityTitle(property, value, out var title))
            {
                return title;
            }

            var number = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
            return NumberHelper.GroupDigit(number);
        }

        if (value is IFormattable formattable)
            return formattable.ToString(null, CultureInfo.CurrentCulture) ?? string.Empty;

        return value.ToString() ?? string.Empty;
    }

    private bool TryGetEntityTitle(PropertyInfo property, object? value, out string title)
    {
        var navigationName = property.Name[..^2];
        var navigationProperty = property.DeclaringType?.GetProperty(navigationName);

        if (navigationProperty is null || value is null)
        {
            title = string.Empty;
            return false;
        }

        var navigationType = Nullable.GetUnderlyingType(navigationProperty.PropertyType) ?? navigationProperty.PropertyType;
        var entityInfo = _entityInfos.FirstOrDefault(info => info.Type == navigationType);

        if (entityInfo is null)
        {
            title = string.Empty;
            return false;
        }

        var id = Convert.ToInt32(value, CultureInfo.InvariantCulture);
        title = entityInfo.GetTitle(id) ?? string.Empty;
        return !string.IsNullOrWhiteSpace(title);
    }

    private static bool TryConvertDecimal(object? value, out decimal? result)
    {
        if (value is null)
        {
            result = null;
            return true;
        }

        try
        {
            result = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            return true;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
        catch (InvalidCastException)
        {
            result = null;
            return false;
        }
        catch (OverflowException)
        {
            result = null;
            return false;
        }
    }

    private static string StripHtml(string? value)
    {
        return string.IsNullOrEmpty(value) ? string.Empty : HtmlTagRegex.Replace(value, string.Empty);
    }
}
