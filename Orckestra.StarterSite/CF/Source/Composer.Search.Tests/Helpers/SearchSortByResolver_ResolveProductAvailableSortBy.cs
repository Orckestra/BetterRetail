//using System;
//using System.Globalization;
//using System.Linq;
//using FizzWare.NBuilder.Generators;
//using FluentAssertions;
//using NUnit.Framework;
//using Orckestra.Composer.Parameters;
//using Orckestra.Composer.Search.Helpers;
//using Orckestra.Composer.Search.Parameters;
//using Orckestra.Composer.Search.Repositories;
//using Orckestra.Composer.Search.ViewModels;
//using Orckestra.Composer.ViewModels;
//using Moq;
//using Orckestra.Composer.Providers;
//using Moq.AutoMock;
//using Orckestra.Composer.Providers.Localization;
//using Orckestra.Composer.Search.Providers;

//namespace Orckestra.Composer.Search.Tests.Helpers
//{
//	[TestFixture]
//	public class SearchSortByResolver_ResolveProductAvailableSortBy
//	{
//		private AutoMocker _container;

//		[SetUp]
//		public void SetUp()
//		{
//			_container = new AutoMocker();
//		}


//		[Test]
//		public void WHEN_Passing_Valid_Parameters_SHOULD_Resolve_Configured_AvailableSortBys()
//		{
//			//Arrange
//			_container.Use(GetLocalizationProviderWithKeynameAsDisplayName());
//			_container.Use(GetSearchUrlProvider());
//			SearchSortByResolver resolver = _container.CreateInstance<SearchSortByResolver>();

//			ProductSearchResultsViewModel model = new ProductSearchResultsViewModel();

//			//Act
//			resolver.Resolve(model, new SearchCriteria {
//				CultureInfo =  CultureInfo.GetCultureInfo("fr-CA"),
//				Scope       = GetRandom.String(32),
//			});

//			//Assert
//			//TODO follow todo in Orckestra.Composer.Search.Helpers.SearchSortByResolver
//			//TODO to assert from the list in SearchConfiguration
//			model.AvailableSortBys.Should().Contain(by => by.DisplayName.Equals("SortByRelavanceLabel"));
//			model.AvailableSortBys.Should().Contain(by => by.DisplayName.Equals("DisplayName_Sort-asc"));
//			model.AvailableSortBys.Should().Contain(by => by.DisplayName.Equals("DisplayName_Sort-desc"));
//			model.AvailableSortBys.Should().Contain(by => by.DisplayName.Equals("ListPrice-asc"));
//			model.AvailableSortBys.Should().Contain(by => by.DisplayName.Equals("ListPrice-desc"));
//		}

//		[Test]
//		public void WHEN_Passing_Valid_Parameters_AvailableSortBys_SHOULD_Have_Urls_With_SortBy_Equals_To_Field()
//		{
//			//Arrange
//			_container.Use(GetLocalizationProviderWithKeynameAsDisplayName());
//			_container.Use(GetSearchUrlProvider());
//			SearchSortByResolver resolver = _container.CreateInstance<SearchSortByResolver>();

//			ProductSearchResultsViewModel model = new ProductSearchResultsViewModel();

//			//Act
//			resolver.Resolve(model, new SearchCriteria
//			{
//				CultureInfo = CultureInfo.InvariantCulture,
//				Scope = GetRandom.String(32),
//			});

//			//Assert
//			model.AvailableSortBys
//				 .Where(by => !by.DisplayName.Equals("SortByRelavanceLabel")) //Omit default sort (relavance)
//				 .Select(by => new {
//					 Field     = by.DisplayName.Split('-').Skip(0).FirstOrDefault(),
//					 Url       = by.Url
//				 }).Where(by => !by.Url.Contains(String.Format("sortBy={0}", by.Field)))
//				 .Should()
//				 .BeEmpty("All url should be sorted by the requested field.");
//		}

//		[Test]
//		public void WHEN_Passing_Valid_Parameters_AvailableSortBys_SHOULD_Have_Urls_With_SortDirection_Equals_To_Field()
//		{
//			//Arrange
//			_container.Use(GetLocalizationProviderWithKeynameAsDisplayName());
//			_container.Use(GetSearchUrlProvider());
//			SearchSortByResolver resolver = _container.CreateInstance<SearchSortByResolver>();

//			ProductSearchResultsViewModel model = new ProductSearchResultsViewModel();

//			//Act
//			resolver.Resolve(model, new SearchCriteria
//			{
//				CultureInfo = CultureInfo.InvariantCulture,
//				Scope = GetRandom.String(32),
//			});

//			//Assert
//			model.AvailableSortBys
//				 .Where(by => !by.DisplayName.Equals("SortByRelavanceLabel")) //Omit default sort (relavance)
//				 .Select(by => new
//				 {
//					 Direction = by.DisplayName.Split('-').Skip(1).FirstOrDefault(),
//					 Url = by.Url
//				 }).Where(by => !by.Url.Contains(String.Format("sortDirection={0}", by.Direction)))
//				 .Should()
//				 .BeEmpty("All url should be sorted by the requested field.");
//		}



//		private Mock<ILocalizationProvider> GetLocalizationProviderWithKeynameAsDisplayName()
//		{
//			Mock<ILocalizationProvider> localizationProvider = new Mock<ILocalizationProvider>();

//			localizationProvider.Setup(
//				lp => lp.GetLocalizedString(It.IsNotNull<GetLocalizedParam>()))
//				.Returns<GetLocalizedParam>(param => String.Format("{0}", param.Key))
//				.Verifiable();

//			return localizationProvider;
//		}

//		private Mock<ISearchUrlProvider> GetSearchUrlProvider()
//		{
//			Mock<ISearchUrlProvider> searchUrlProvider = new Mock<ISearchUrlProvider>();

//			searchUrlProvider.Setup(
//				p => p.BuildSearchUrl(It.IsNotNull<SearchCriteria>()))
//				.Returns<SearchCriteria>(sc => String.Format("sortBy={0}&sortDirection={1}", sc.SortBy, sc.SortDirection))
//				.Verifiable();

//			return searchUrlProvider;
//		}
//	}
//}
