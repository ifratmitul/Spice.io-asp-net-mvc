using Microsoft.AspNetCore.Mvc.Rendering;

namespace Spice.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {

                       Text = item.GetPropertyValue("Name").Trim(),
                       Value = item.GetPropertyValue("Id").Trim(),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())

                   };

        }
    }
}
