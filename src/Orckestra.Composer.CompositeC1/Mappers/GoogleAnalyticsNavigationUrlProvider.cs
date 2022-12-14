using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;
using System.Web;
using Orckestra.Composer.Utils;
using System.Collections.Specialized;
using Orckestra.Composer.CompositeC1.Providers.MainMenu;

namespace Orckestra.Composer.CompositeC1.Mappers
{
    public class GoogleAnalyticsNavigationUrlProvider
    {
        public enum MenuOrigin
        {
            Dropdown,
            Sticky,
            Footer
        }

        protected const string Origin_QueryStringParam = "origin";
        protected const string ClickedOn_QueryStringParam = "clickedon";
        protected const string CategoryPattern_QueryStringParam = "c{0}";

        private static readonly ConcurrentDictionary<string, string> _sanitizedDisplayNameCache = new ConcurrentDictionary<string, string>();

        protected virtual string SanitizeDisplayName(string displayName)
        {
            return _sanitizedDisplayNameCache.GetOrAdd(displayName, str => HttpUtility.UrlEncode(UrlFormatter.FormatProductName(str)));
        }

        protected virtual string BuildUrl(string url, string displayName, NameValueCollection categoryHierarchy, MenuOrigin origin)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            var queryString = new NameValueCollection();

            queryString.Add(Origin_QueryStringParam, origin.ToString().ToLowerInvariant());
            queryString.Add(categoryHierarchy);
            queryString.Add(ClickedOn_QueryStringParam, SanitizeDisplayName(displayName));

            return UrlFormatter.AppendQueryString(url, queryString);
        }

        public virtual string BuildUrl(MainMenuItemWrapper current, Dictionary<Guid, MainMenuItemWrapper> items, MenuOrigin origin)
        {
            var categoryHierarchy = GetCategoryHierarchy(items, current);
            return BuildUrl(current.Url, current.DisplayName, categoryHierarchy, origin);
        }

        public virtual string BuildUrl(HeaderOptionalLink current, MenuOrigin origin)
        {
            var categoryHierarchy = GetCategoryHierarchy(current);
            return BuildUrl(current.Url, current.DisplayName, categoryHierarchy, origin);
        }

        public virtual string BuildUrl(FooterOptionalLink current, MenuOrigin origin)
        {
            var categoryHierarchy = GetCategoryHierarchy(current);
            return BuildUrl(current.Url, current.DisplayName, categoryHierarchy, origin);
        }

        public virtual string BuildUrl(Footer current, IEnumerable<Footer> items, MenuOrigin origin)
        {
            var categoryHierarchy = GetCategoryHierarchy(items, current);
            return BuildUrl(current.Url, current.DisplayName, categoryHierarchy, origin);
        }

        public virtual string BuildUrl(StickyHeader current, MenuOrigin origin)
        {
            var categoryHierarchy = GetCategoryHierarchy(current);
            return BuildUrl(current.Url, current.DisplayName, categoryHierarchy, origin);
        }

        /// <summary>
        /// Get the hierarchy of a MainMenu collection from C1
        /// </summary>
        /// <param name="items"></param>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        private IEnumerable<string> GetHierarchy(Dictionary<Guid, MainMenuItemWrapper> items, MainMenuItemWrapper currentItem)
        {
            yield return SanitizeDisplayName(currentItem.DisplayName);

            Guid? currentMenuItemId = currentItem.ParentId;
            while (currentMenuItemId.HasValue)
            {
                var item = items[currentMenuItemId.Value];
                yield return SanitizeDisplayName(item.DisplayName);

                currentMenuItemId = item.ParentId;
            }
        }

        protected virtual NameValueCollection GetCategoryHierarchy(Dictionary<Guid, MainMenuItemWrapper> items, MainMenuItemWrapper currentItem)
        {
            var hierarchy = GetHierarchy(items, currentItem).Reverse().ToArray();

            return BuildCategoryHierarchyQueryStringParameters(hierarchy);
        }

        /// <summary>
        /// Get the hierarchy of a Footer collection from C1
        /// </summary>
        /// <param name="items"></param>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        private IEnumerable<string> GetHierarchy(IEnumerable<Footer> items, Footer currentItem)
        {
            yield return SanitizeDisplayName(currentItem.DisplayName);

            while (currentItem.ParentId.HasValue)
            {
                var parent = items.First(x => x.Id.Equals(currentItem.ParentId.Value));
                yield return SanitizeDisplayName(parent.DisplayName);
                currentItem = parent;
            }
        }

        protected virtual NameValueCollection GetCategoryHierarchy(IEnumerable<Footer> items, Footer currentItem)
        {
            var collection = new NameValueCollection();
            var hierarchy = GetHierarchy(items, currentItem).Reverse().ToArray();

            return BuildCategoryHierarchyQueryStringParameters(hierarchy);
        }

        /// <summary>
        /// StickyHeader does not contains sub items so I just return the c1 level 
        /// </summary>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        protected virtual NameValueCollection GetCategoryHierarchy(StickyHeader currentItem)
        {
            return BuildCategoryHierarchyQueryStringParameters(SanitizeDisplayName(currentItem.DisplayName));
        }

        /// <summary>
        /// FooterOptionalLink does not contains sub items so I just return the c1 level 
        /// </summary>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        protected virtual NameValueCollection GetCategoryHierarchy(FooterOptionalLink currentItem)
        {
            return BuildCategoryHierarchyQueryStringParameters(SanitizeDisplayName(currentItem.DisplayName));
        }

        /// <summary>
        /// HeaderOptionalLink does not contains sub items so I just return the c1 level 
        /// </summary>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        protected virtual NameValueCollection GetCategoryHierarchy(HeaderOptionalLink currentItem)
        {
            return BuildCategoryHierarchyQueryStringParameters(SanitizeDisplayName(currentItem.DisplayName));
        }

        protected NameValueCollection BuildCategoryHierarchyQueryStringParameters(params string[] @params)
        {
            var collection = new NameValueCollection();
            var index = 1;

            foreach (var param in @params)
            {
                collection.Add(string.Format(CategoryPattern_QueryStringParam, index), param);
                index++;
            }

            return collection;
        }
    }
}
