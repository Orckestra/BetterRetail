using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Orckestra.Composer.Search.Repositories
{
	/// <summary>
	/// The repository call to the overture client
	/// </summary>
	public class SearchManagementRepository : ISearchManagementRepository
	{
		List<SearchManagementScope> searchManagementScopes { get; set; } = new List<SearchManagementScope>();

		public SearchManagementRepository()
		{
			initializeSearchKeywordValues();
			initializeSearchSuggestionValues();
		}

		public Task<string> GetSearchKeywordRedirect(string scope, CultureInfo cultureInfo, string keyword)
		{
			string redirectUrl = null;
			SearchManagementCulture smCulture = FindForScopeAndCulture(scope, cultureInfo);
			if (smCulture != null)
			{
				redirectUrl = smCulture.keywordRedirects.ContainsKey(keyword) ? smCulture.keywordRedirects[keyword] : null;
			}

            return Task.FromResult(redirectUrl);
        }

		public Task<List<string>> GetSearchSuggestedTerms(string scope, CultureInfo cultureInfo, string keyword)
		{
			List<string> suggestions = new List<string>();

			SearchManagementCulture smCulture = FindForScopeAndCulture(scope, cultureInfo);
			if (smCulture != null)
			{
				if (smCulture.suggestedTerms != null )
				{
					string keywordLower = keyword.ToLower();
					suggestions = smCulture.suggestedTerms.Where(suggestion => suggestion.ToLower().Contains(keywordLower)).ToList();
				}
			}

			return Task.FromResult(suggestions);
		}

		private SearchManagementCulture FindForScopeAndCulture(string scope, CultureInfo cultureInfo)
		{
			SearchManagementScope smScope = searchManagementScopes.FirstOrDefault(item => item.Scope.Equals(scope));
			if (smScope != null)
			{
				SearchManagementCulture smCulture = smScope.cultures.FirstOrDefault(item => item.cultureInfo.Equals(cultureInfo));
				if (smCulture != null)
				{
					return smCulture;
				}
			}

			return null;
		}

		#region Hardcoded Values
		private void initializeSearchKeywordValues()
		{
			SearchManagementCulture cultureEnUS = new SearchManagementCulture { cultureInfo = new CultureInfo("en-US") };
			cultureEnUS.keywordRedirects.Add("secure", "/en-us/secure-content");
			cultureEnUS.keywordRedirects.Add("meter", "/en-US/p-meter-portable-mf-pro-ott/PIM-236350");

			SearchManagementCulture cultureEnGB = new SearchManagementCulture { cultureInfo = new CultureInfo("en-US") };
			cultureEnGB.keywordRedirects.Add("cart", "/en-US/Cart");
			cultureEnGB.keywordRedirects.Add("find", "/en-US/Stores?origin=dropdown&c1=where-to-find-us&clickedon=where-to-find-us");

			SearchManagementScope scopeUS = new SearchManagementScope { Scope = "OTT_US" };
			scopeUS.cultures.Add(cultureEnUS);
			scopeUS.cultures.Add(cultureEnGB);

			searchManagementScopes.Add(scopeUS);
		}

		private void initializeSearchSuggestionValues()
		{
			SearchManagementCulture smCulture = FindForScopeAndCulture("OTT_US", new CultureInfo("en-US"));
			if (smCulture != null)
			{
				smCulture.suggestedTerms = new List<string>
				{
					"Uv","4035","17491LM","HX02BDL","sl-10a","toc","3050","3354","manuals","16518-1","3198","HX02DDS",
					"parts","16518","7305","dw-8","41114-2","HX02DDL","CSL-8R","s-254","11820","18059","4125","mp2sl",
					"16676","dw-300","HX02CDS","uv lamps","HX06CDL","trojan","793923","sl","sl1","3087","3098","ozone",
					"122-1","17998","3101","4253","csl-12r","quartz sleeve","12118","dw","dw8","hx02cdlu","sp-2","18063",
					"3010","41114","Pro Velocity","Ecocap", "connecting cable", "modem",
				};
			}
		}
		#endregion

		private class SearchManagementScope
		{
			public string Scope { get; set; }
			public List<SearchManagementCulture> cultures { get; set; } = new List<SearchManagementCulture>();
		}

		private class SearchManagementCulture
		{
			public CultureInfo cultureInfo { get; set; }
			public Dictionary<string, string> keywordRedirects { get; set; } = new Dictionary<string, string>();
			public List<string> suggestedTerms { get; set; } = new List<string>();
		}
	}
}