using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Composite.Core;
using Composite.Core.Configuration;
using Composite.Core.Extensions;
using Composite.Core.Linq;
using Composite.Data;
using Composite.Data.ProcessControlled;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;

namespace Orckestra.Composer.CompositeC1.Services
{
	public static class CompositeLanguageFacade
	{
		public static T GetTranslationSource<T>(this T dataFromAdministratedScope) where T : class, IData
		{
			IPublishControlled publishControlled = dataFromAdministratedScope as IPublishControlled;

			if (!GlobalSettingsFacade.OnlyTranslateWhenApproved
				|| publishControlled == null
				|| publishControlled.PublicationStatus == GenericPublishProcessController.AwaitingPublication)
			{
				return dataFromAdministratedScope;
			}

			using (new DataScope(dataFromAdministratedScope.DataSourceId.LocaleScope))
			{
				return (DataFacade.GetDataFromOtherScope(dataFromAdministratedScope as IData, DataScopeIdentifier.Public)
								  .FirstOrDefault()
						?? dataFromAdministratedScope) as T;
			}
		}


		public static IPage TranslatePage(IPage sourcePage, CultureInfo sourceCultureInfo, CultureInfo targetCultureInfo)
		{
			IPage newPage;

			using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{

				List<IPagePlaceholderContent> sourcePagePlaceholders;
				List<IData> sourceMetaDataSet;

				using (new DataScope(sourceCultureInfo))
				{

					using (new DataScope(DataScopeIdentifier.Administrated))
					{
						sourcePage = sourcePage.GetTranslationSource();

						using (new DataScope(sourcePage.DataSourceId.DataScopeIdentifier))
						{
							sourcePagePlaceholders = DataFacade.GetData<IPagePlaceholderContent>(f => f.PageId == sourcePage.Id).ToList();
							sourceMetaDataSet = sourcePage.GetMetaData().ToList();
						}
					}
				}

				using (new DataScope(targetCultureInfo))
				{
					newPage = DataFacade.BuildNew<IPage>();
					sourcePage.ProjectedCopyTo(newPage);

					newPage.SourceCultureName = targetCultureInfo.Name;
					newPage.PublicationStatus = GenericPublishProcessController.Draft;
					newPage = DataFacade.AddNew<IPage>(newPage);

					foreach (IPagePlaceholderContent sourcePagePlaceholderContent in sourcePagePlaceholders)
					{
						IPagePlaceholderContent newPagePlaceholderContent = DataFacade.BuildNew<IPagePlaceholderContent>();
						sourcePagePlaceholderContent.ProjectedCopyTo(newPagePlaceholderContent);

						newPagePlaceholderContent.SourceCultureName = targetCultureInfo.Name;
						newPagePlaceholderContent.PublicationStatus = GenericPublishProcessController.Draft;
						DataFacade.AddNew<IPagePlaceholderContent>(newPagePlaceholderContent);
					}

					foreach (IData metaData in sourceMetaDataSet)
					{
						ILocalizedControlled localizedData = metaData as ILocalizedControlled;

						if (localizedData == null)
						{
							continue;
						}

						IEnumerable<ReferenceFailingPropertyInfo> referenceFailingPropertyInfos = DataLocalizationFacade.GetReferencingLocalizeFailingProperties(localizedData).Evaluate();

						if (!referenceFailingPropertyInfos.Any())
						{
							IData newMetaData = DataFacade.BuildNew(metaData.DataSourceId.InterfaceType);
							metaData.ProjectedCopyTo(newMetaData);

							ILocalizedControlled localizedControlled = newMetaData as ILocalizedControlled;
							localizedControlled.SourceCultureName = targetCultureInfo.Name;

							IPublishControlled publishControlled = newMetaData as IPublishControlled;
							publishControlled.PublicationStatus = GenericPublishProcessController.Draft;

							DataFacade.AddNew(newMetaData);
						}
						else
						{
							foreach (ReferenceFailingPropertyInfo referenceFailingPropertyInfo in referenceFailingPropertyInfos)
							{
								Log.LogVerbose("LocalizePageWorkflow",
												"Meta data of type '{0}' is not localized because the field '{1}' is referring some not yet localzed data"
												.FormatWith(metaData.DataSourceId.InterfaceType, referenceFailingPropertyInfo.DataFieldDescriptor.Name));
							}
						}
					}
				}

		

				transactionScope.Complete();
			}
			return newPage;
		}
	}
}
