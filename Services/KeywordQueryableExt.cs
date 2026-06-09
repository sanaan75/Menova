using System.Linq.Expressions;
using Entities;

namespace Services;

// todo : move to services.common
public static class KeywordQueryableExt
{
    public static IQueryable<T> FilterByKeyword<T>(this IQueryable<T> items, string keyword, Func<string, Expression<Func<T, bool>>> predicateBuilder)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return items;

        var tokens = keyword.GetTokens();

        foreach (var t in tokens)
            items = items.Where(predicateBuilder(t));

        return items;
    }
}