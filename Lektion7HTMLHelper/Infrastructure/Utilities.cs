using Lektion7HTMLHelper.Models;

namespace Lektion7HTMLHelper.Infrastructure;

public static class Utilities
{
    public static List<CountryItem> SortCountryList(
        List<CountryItem> countries)
    {
        return countries
            .OrderBy(c => c.Name)
            .ToList();
    }
}