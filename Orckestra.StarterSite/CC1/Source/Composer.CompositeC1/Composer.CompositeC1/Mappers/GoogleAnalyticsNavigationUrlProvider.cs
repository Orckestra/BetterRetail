using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Composite.Data;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels.Home;
using Orckestra.Composer.ViewModels.MenuNavigation;
using System.Web;
using Orckestra.Composer.Utils;
using System.Collections.Specialized;

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

        protected virtual string SanatizeDisplayName(string displayName)
        {
            return HttpUtility.UrlEncode(UrlFormatter.FormatProductName(displayName.ToLower()));
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
            queryString.Add(ClickedOn_QueryStringParam, SanatizeDisplayName(displayName));

            return UrlFormatter.AppendQueryString(url, queryString);
        }

        public virtual string BuildUrl(MainMenu current, IEnumerable<MainMenu> items, MenuOrigin origin)
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
        private IEnumerable<string> GetHierarchy(IEnumerable<MainMenu> items, MainMenu currentItem)
        {            
            yield return SanatizeDisplayName(currentItem.DisplayName);            

            while (currentItem.ParentId.HasValue)
            {
                var parent = items.First(x => x.Id.Equals(currentItem.ParentId.Value));
                yield return SanatizeDisplayName(parent.DisplayName);
                currentItem = parent;
            }
        }

        protected virtual NameValueCollection GetCategoryHierarchy(IEnumerable<MainMenu> items, MainMenu currentItem)
        {
            var collection = new NameValueCollection();
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
            yield return SanatizeDisplayName(currentItem.DisplayName);            

            while (currentItem.ParentId.HasValue)
            {
                var parent = items.First(x => x.Id.Equals(currentItem.ParentId.Value));
                yield return SanatizeDisplayName(parent.DisplayName);
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
            return BuildCategoryHierarchyQueryStringParameters(SanatizeDisplayName(currentItem.DisplayName));
        }

        /// <summary>
        /// FooterOptionalLink does not contains sub items so I just return the c1 level 
        /// </summary>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        protected virtual NameValueCollection GetCategoryHierarchy(FooterOptionalLink currentItem)
        {
            return BuildCategoryHierarchyQueryStringParameters(SanatizeDisplayName(currentItem.DisplayName));
        }

        /// <summary>
        /// HeaderOptionalLink does not contains sub items so I just return the c1 level 
        /// </summary>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        protected virtual NameValueCollection GetCategoryHierarchy(HeaderOptionalLink currentItem)
        {
            return BuildCategoryHierarchyQueryStringParameters(SanatizeDisplayName(currentItem.DisplayName));
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
