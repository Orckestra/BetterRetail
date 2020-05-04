using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Composite.Core.Linq;
using Composite.Data;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration.DataTypes;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.CompositeC1.Services
{
	public class CategoryPageService : ICategoryBrowsingService
	{
		public readonly Guid RootPageId;

        protected ICategoryRepository CategoryRepository { get; private set; }
        protected IScopeProvider ScopeProvider { get; private set; }
        public ILocalizationProvider LocalizationProvider { get; set; }
        public ICultureService CultureService { get; set; }

        public CategoryPageService(
            ICategoryRepository categoryRepository,
            IScopeProvider scopeProvider, 
            ILocalizationProvider localizationProvider,
            ICultureService cultureService)
		{
			ScopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
			LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
			CategoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            CultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));

	        RootPageId = (Guid) CategoriesConfiguration.CategoriesSyncConfiguration["RootPageId"];
		}

		public void Sync()
		{
		    var categories = CategoryRepository
		        .GetCategoriesTreeAsync(new GetCategoriesParam {Scope = ScopeProvider.DefaultScope}).Result;
			
            CategoryPageData categoryPageData = GetCategoryPages();
			EnsureCategoryPages(categories["Root"], categoryPageData);
		}

		protected virtual void EnsureCategoryPages(TreeNode<Category> rootCategory, CategoryPageData pageData)
		{
			foreach (var childCategory in rootCategory.Children)
			{
				var category = childCategory;
				var data = pageData;
				EnsureSubCategoryPages(category, data);
			}
		}

        protected virtual void EnsureSubCategoryPages(TreeNode<Category> subCategory, CategoryPageData pageData)
		{
            var isLandingPage = subCategory.GetLevel() <= CategoriesConfiguration.LandingPageMaxLevel;
            var pageTypeId = isLandingPage ? CategoryPages.CategoryLandingPageTypeId : CategoryPages.CategoryPageTypeId;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				IPage source = null;

				var sourceData = pageData.Default;
				if (!CategoryExist(subCategory.Value, sourceData))
				{
					using (var connection = new DataConnection(PublicationScope.Unpublished, pageData.Default.Culture))
					{
						// Get default page content
						// TODO: Do it only once

						var pageTypeDefaultPageContents =
							DataFacade.GetData<IPageTypeDefaultPageContent>().
								Where(f => f.PageTypeId == pageTypeId).
								Evaluate();

						// Add page to parent
						var catPage = connection.CreateNew<IPage>();
						FillCategoryPage(catPage, pageTypeId, subCategory, connection.CurrentLocale);
						var parentParentId = subCategory.GetLevel() == 1
							? RootPageId
							: GenerateCategoryPageId(subCategory.Parent.Value.Id);

						catPage.AddPageAtBottom(parentParentId);

						// Add ComposerCategoryPage MetaData info
						AddCategoryPageInfo(subCategory, connection, catPage, isLandingPage, false);

						// Add default page content
						foreach (IPageTypeDefaultPageContent pageTypeDefaultPageContent in pageTypeDefaultPageContents)
						{
							IPagePlaceholderContent pagePlaceholderContent = DataFacade.BuildNew<IPagePlaceholderContent>();
							pagePlaceholderContent.PageId = catPage.Id;
							pagePlaceholderContent.PlaceHolderId = pageTypeDefaultPageContent.PlaceHolderId;
							pagePlaceholderContent.Content = pageTypeDefaultPageContent.Content;
							DataFacade.AddNew<IPagePlaceholderContent>(pagePlaceholderContent);
						}

						// Update page again to force publish
						var addedPage = DataFacade.GetData<IPage>(p => p.Id == catPage.Id);

						DataFacade.Update(addedPage);
					}
				}

				foreach (var localizedData in pageData.Others)
				{
					if (!CategoryExist(subCategory.Value, localizedData))
					{
						using (var locConnection = new DataConnection(PublicationScope.Unpublished, localizedData.Culture))
						{
							if (source == null)
							{
								var sourceId = GenerateCategoryPageId(subCategory.Value.Id);
								using (var connection = new DataConnection(PublicationScope.Unpublished, pageData.Default.Culture))
								{
									source = connection.Get<IPage>().FirstOrDefault(p => p.Id == sourceId);
								}
								if (source == null)
								{
									//TODO: Log
									continue;
								}
								var localizedPage = CompositeLanguageFacade.TranslatePage(source, pageData.Default.Culture, localizedData.Culture);
								FillCategoryPage(localizedPage, pageTypeId, subCategory, localizedData.Culture);
								AddCategoryPageInfo(subCategory, locConnection, localizedPage, isLandingPage, false);
								locConnection.Update(localizedPage);
							}
						}
					}
				}
				transaction.Complete();
			}

			if (subCategory.Children == null) return;

			if (isLandingPage)
			{
				CreateCategoryAllProductsPage(subCategory, pageData);
			}

			foreach (var child in subCategory.Children)
			{
				EnsureSubCategoryPages(child, pageData);
			}
		}

        protected virtual void CreateCategoryAllProductsPage(TreeNode<Category> subCategory, CategoryPageData pageData)
		{
			using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				IPage source = null;

				var sourceData = pageData.Default;
				if (!CategoryAllProductsExist(subCategory.Value, sourceData))
				{
					using (var connection = new DataConnection(PublicationScope.Unpublished, pageData.Default.Culture))
					{
						// Get default page content
						// TODO: Do it only once
						var selectedPageType = DataFacade.GetData<IPageType>().Single(f => f.Id == CategoryPages.CategoryPageTypeId);
						var pageTypeDefaultPageContents =
							DataFacade.GetData<IPageTypeDefaultPageContent>().
								Where(f => f.PageTypeId == selectedPageType.Id).
								Evaluate();

						// Add page to parent
						var catPage = connection.CreateNew<IPage>();
						FillCategoryAllProductsPage(catPage, subCategory, connection.CurrentLocale);
						var parentId = GenerateCategoryPageId(subCategory.Value.Id);

						catPage.AddPageAtBottom(parentId);

						// Add ComposerCategoryPage MetaData info
						AddCategoryPageInfo(subCategory, connection, catPage, false, true);

						// Add default page content
						foreach (IPageTypeDefaultPageContent pageTypeDefaultPageContent in pageTypeDefaultPageContents)
						{
							IPagePlaceholderContent pagePlaceholderContent = DataFacade.BuildNew<IPagePlaceholderContent>();
							pagePlaceholderContent.PageId = catPage.Id;
							pagePlaceholderContent.PlaceHolderId = pageTypeDefaultPageContent.PlaceHolderId;
							pagePlaceholderContent.Content = pageTypeDefaultPageContent.Content;
							DataFacade.AddNew<IPagePlaceholderContent>(pagePlaceholderContent);
						}

						// Update page again to force publish
						var addedPage = DataFacade.GetData<IPage>(p => p.Id == catPage.Id);
						DataFacade.Update(addedPage);
					}
				}

				foreach (var localizedData in pageData.Others)
				{
					if (!CategoryAllProductsExist(subCategory.Value, localizedData))
					{
						using (var locConnection = new DataConnection(PublicationScope.Unpublished, localizedData.Culture))
						{
							if (source == null)
							{
								var sourceId = GenerateCategoryAllProductsPageId(subCategory.Value.Id);
								using (var connection = new DataConnection(PublicationScope.Unpublished, pageData.Default.Culture))
								{
									source = connection.Get<IPage>().FirstOrDefault(p => p.Id == sourceId);
								}
								if (source == null)
								{
									//TODO: Log
									continue;
								}
								var localizedPage = CompositeLanguageFacade.TranslatePage(source, pageData.Default.Culture, localizedData.Culture);
								AddCategoryPageInfo(subCategory, locConnection, localizedPage, false, true);
								FillCategoryAllProductsPage(localizedPage, subCategory, localizedData.Culture);
								locConnection.Update(localizedPage);
							}
						}
					}
				}
				transaction.Complete();
			}
		}

		protected static void AddCategoryPageInfo(TreeNode<Category> subCategory, DataConnection connection, IPage catPage, bool isLandingPage, bool isAllProductsPage)
		{
			var categoryPageInfo = connection.CreateNew<CategoryPage>();
			categoryPageInfo.CategoryId = subCategory.Value.Id;
			categoryPageInfo.IsAllProductsPage = isAllProductsPage;

            var metaDefName = isLandingPage ? "ComposerCategoryLandingPage" : "ComposerCategoryPage";
			catPage.AddNewMetaDataToExistingPage(metaDefName, typeof (CategoryPage), categoryPageInfo);
		}
        
		protected bool IsDefaultLanguage(CultureInfo culture)
		{
			return CultureService.GetDefaultCulture().Name == culture.Name;
		}

		protected virtual void FillCategoryPage(IPage page, Guid pageTypeId, TreeNode<Category> category, CultureInfo culture)
		{
			var pageId = GenerateCategoryPageId(category.Value.Id);
			page.Id = pageId;
			page.PageTypeId = pageTypeId;
			page.TemplateId = GetCategoryPageTemplatedId(category);
			page.Title = category.Value.DisplayName.GetLocalizedValue(culture.Name);
			page.MenuTitle = category.Value.DisplayName.GetLocalizedValue(culture.Name); ;
			page.UrlTitle = UrlFormatter.FormatProductName(category.Value.DisplayName.GetLocalizedValue(culture.Name));
			page.PublicationStatus = GenericPublishProcessController.Published;
		}

		protected virtual void FillCategoryAllProductsPage(IPage page, TreeNode<Category> category, CultureInfo culture)
		{
			var pageId = GenerateCategoryAllProductsPageId(category.Value.Id);
			page.Id = pageId;
			page.PageTypeId = CategoryPages.CategoryPageTypeId;
			page.TemplateId = GetCategoryPageTemplatedId(category, true);
			var title = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
			{
				Category = "List-Search",
				Key = "L_AllProduct_URL",
				CultureInfo = culture
			});
			page.Title = title;
			page.MenuTitle = title;
			page.UrlTitle = UrlFormatter.FormatProductName(title);
			page.PublicationStatus = GenericPublishProcessController.Published;
		}

		protected virtual Guid GetCategoryPageTemplatedId(TreeNode<Category> category, bool isAllProducts = false) // TODO
		{
			return (category.GetLevel() <= CategoriesConfiguration.LandingPageMaxLevel && !isAllProducts) 
                ? CategoryPages.CategoryTopicPageTemplateId 
                : CategoryPages.CategoryAllProductsPageTemplateId;
		}

        protected virtual bool CategoryExist(Category category, CategoryLocalizedPageData categoryPageData)
		{
			var catId = GenerateCategoryPageId(category.Id);
			return categoryPageData.Pages.ContainsKey(catId);
		}

        protected virtual bool CategoryAllProductsExist(Category category, CategoryLocalizedPageData categoryPageData)
		{
			var catId = GenerateCategoryAllProductsPageId(category.Id);
			return categoryPageData.Pages.ContainsKey(catId);
		}

        protected virtual CategoryPageData GetCategoryPages()
		{
			var result = new CategoryPageData
			{
				Others = new List<CategoryLocalizedPageData>()
			};

			foreach (var culture in GetCultures())
			{
			    var catInfo = GetCategoryLocalizedPageData(culture);

                if (IsDefaultLanguage(culture))
                {
                    result.Default = catInfo;
                }
                else
                {
                    result.Others.Add(catInfo);
                }
            }
			return result;
		}


        protected static CategoryLocalizedPageData GetCategoryLocalizedPageData(CultureInfo culture)
        {
            using (var connection = new DataConnection(PublicationScope.Unpublished, culture))
            {
                return new CategoryLocalizedPageData
                {
                    Pages = connection.Get<IPage>()
                        .Where(p => p.PageTypeId == CategoryPages.CategoryPageTypeId
                                    || p.PageTypeId == CategoryPages.CategoryLandingPageTypeId)
                        .ToDictionary(c => c.Id, c => c.Id),
                    Culture = culture,
                };
            }
        }

        public virtual Guid GenerateCategoryPageId(string categoryId)
		{
			return GuidUtility.Create(CategoriesNamespaces.CategoryNamespaceId, categoryId);
		}

		public virtual Guid GenerateCategoryAllProductsPageId(string categoryId)
		{
			return GuidUtility.Create(CategoriesNamespaces.AllProductsNamespaceId, categoryId);
		}

		public void Clear()
		{
			foreach (var culture in GetCultures())
			{
				using (DataConnection connection = new DataConnection(PublicationScope.Unpublished, culture))
				{
					var root = connection.Get<IPage>().FirstOrDefault(p => p.Id == RootPageId);
					if (root == null)
					{
						return;
					}

					var itemToDeletes = root.GetChildren().Where(p => p.PageTypeId == CategoryPages.CategoryPageTypeId || p.PageTypeId == CategoryPages.CategoryLandingPageTypeId).ToArray();
					foreach (var page in itemToDeletes)
					{
						DeleteChildrenCategories(page);
					}
				}
			}
		}

        protected virtual void DeleteChildrenCategories(IPage page)
		{
			var children = page.GetChildren().Where(p => p.PageTypeId == CategoryPages.CategoryPageTypeId || p.PageTypeId == CategoryPages.CategoryLandingPageTypeId).ToArray();
			foreach (var child in children)
			{
				DeleteChildrenCategories(child);
			}

			//if (!ExistInOtherLocale(GetCultures(), page))
			//	RemoveAllFolderAndMetaDataDefinitions(page);

			page.DeletePageStructure();
			ProcessControllerFacade.FullDelete(page);
		}

        protected virtual IEnumerable<CultureInfo> GetCultures()
		{
            return CultureService.GetAllSupportedCultures();
		}

        protected virtual void RemoveAllFolderAndMetaDataDefinitions(IPage page)
		{
			foreach (Type folderType in page.GetDefinedFolderTypes())
			{
				page.RemoveFolderDefinition(folderType, true);
			}

			foreach (Tuple<Type, string> metaDataTypeAndName in page.GetDefinedMetaDataTypeAndNames())
			{
				page.RemoveMetaDataDefinition(metaDataTypeAndName.Item2, true);
			}
		}

        protected virtual bool ExistInOtherLocale(List<CultureInfo> cultures, IPage page)
		{
			foreach (CultureInfo localeCultureInfo in cultures)
			{
				using (new DataScope(localeCultureInfo))
				{
					if (PageManager.GetPageById(page.Id) != null)
					{
						return true;
					}
				}
			}

			return false;
		}

        protected class CategoryLocalizedPageData
		{
			public CultureInfo Culture { get; set; }

			public Dictionary<Guid, Guid> Pages { get; set; }
		}

		protected class CategoryPageData
		{
			public CategoryLocalizedPageData Default { get; set; }

			public List<CategoryLocalizedPageData> Others { get; set; }
		}
	}
}
