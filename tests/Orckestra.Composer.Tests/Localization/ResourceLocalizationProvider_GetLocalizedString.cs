using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Tests.Mock;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;
using static Orckestra.Composer.Utils.ExpressionUtility;
using System.Linq.Expressions;

namespace Orckestra.Composer.Tests.Localization
{
    [TestFixture]
    public class ResourceLocalizationProviderGetLocalizedString
    {
        private ILocalizationProvider _localizationProvider;

        [SetUp]
        public void SetUp()
        {
            _localizationProvider = LocalizationProviderFactory.CreateFromTestAssets();
        }

        [Test]
        public void WHEN_retreive_french_canadian_SHOULD_return_localized_string()
        {
            string paymentMethodString = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.GetCultureInfo("fr-CA")
                }
            );

            paymentMethodString.Should().NotBeNull();
            paymentMethodString.Should().Be("Québec Page Title");
        }

        [Test]
        public void WHEN_retreive_french_france_SHOULD_return_french_localized_string()
        {
            string paymentMethodString = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.GetCultureInfo("fr-FR")
                }
            );

            paymentMethodString.Should().NotBeNull();
            paymentMethodString.Should().Be("Generic French Page Title");
        }

        [Test]
        public void WHEN_retreive_spanish_mexico_SHOULD_return_neutral_localized_string()
        {
            string paymentMethodString = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.GetCultureInfo("es-MX")
                }
            );

            paymentMethodString.Should().NotBeNull();
            paymentMethodString.Should().Be("Neutral Page Title");
        }

        [Test]
        public void WHEN_retreive_customized_french_from_belgium_SHOULD_return_customized_localized_string()
        {
            string paymentMethodString = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.GetCultureInfo("fr-BE")
                }
            );

            paymentMethodString.Should().NotBeNull();
            paymentMethodString.Should().Be("Customized Belgium Page Title");
        }

        [Test]
        public void WHEN_key_does_not_exists_SHOULD_return_Null()
        {
            string paymentMethodString = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "SomeKeyThatDoesNotExists",
                    CultureInfo = CultureInfo.GetCultureInfo("fr-CA")
                }
            );

            paymentMethodString.Should().BeNull();
        }

        [Test]
        public void WHEN_key_is_null_SHOULD_throw_argument_exception()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                _localizationProvider.GetLocalizedString(new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    CultureInfo = CultureInfo.GetCultureInfo("fr-CA")
                });
            });

            exception.ParamName.Should().BeSameAs("param");
            exception.Message.Should().Contain("Key");
        }

        [Test]
        public void WHEN_category_is_null_SHOULD_throw_invalid_argument_exception()
        {
             var exception = Assert.Throws<ArgumentException>(() =>
             {
                 _localizationProvider.GetLocalizedString(new GetLocalizedParam
                 {
                     Key         = "ResxLocalizationTest",
                     CultureInfo = CultureInfo.GetCultureInfo("fr-CA")
                 });
             });

             exception.ParamName.Should().BeSameAs("param");
             exception.Message.Should().Contain("Category");
        }

        [Test]
        public void WHEN_culture_is_null_SHOULD_throw_invalid_argument_exception()
        {
            var param = new GetLocalizedParam
            {
                Category = "ResxLocalizationTest",
                Key = "PageTitle"
            };

            Expression<Func<string>> expression = () => _localizationProvider.GetLocalizedString(param);
            var exception = Assert.Throws<ArgumentException>(() => expression.Compile().Invoke());

            exception.ParamName.Should().BeEquivalentTo(GetParamsInfo(expression)[0].Name);
            exception.Message.Should().StartWith(GetMessageOfNullWhiteSpace(nameof(param.CultureInfo.Name)));
        }

        [Test]
        [Ignore("The latest versions of API return neutral localizatin strings when the culture is not supported")]
        public void WHEN_culture_is_not_supported_SHOULD_throw_culture_not_supported_exception()
        {
            Assert.Throws<CultureNotFoundException>(() => 
                _localizationProvider.GetLocalizedString(
                    new GetLocalizedParam
                    {
                        Category    = "ResxLocalizationTest",
                        Key         = "PageTitle",
                        CultureInfo = CultureInfo.GetCultureInfo("mx-MX")
                    }
                )
            );
        }

        [Test]
        public void WHEN_category_does_not_exists_SHOULD_return_null()
        {
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "SomeCategoryThatDoesNotExists",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.GetCultureInfo("fr-CA")
                }
            );

            value.Should().BeNull();
        }

        /// <summary>
        /// Case Insensitive on CategoryName
        /// </summary>
        [Test]
        public void WHEN_Category_Case_Doesnt_Match_SHOULD_still_Return_Localized_Value()
        {
            var localizedString = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "rEsXlOcAlIzAtIoNtEsT",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.GetCultureInfo("fr-BE")
                }
            );

            localizedString.Should().NotBeNull();
            localizedString.Should().Be("Customized Belgium Page Title");
        }

        /// <summary>
        /// Case Sensitive on KeyName
        /// </summary>
        [Test]
        public void WHEN_KeyName_Case_Doesnt_Match_SHOULD_Return_null()
        {
            var localizedString = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category = "ResxLocalizationTest",
                    Key = "pAgEtItLe",
                    CultureInfo = CultureInfo.GetCultureInfo("fr-BE")
                }
            );

            localizedString.Should().BeNull();
        }

        [Test]
        public void WHEN_Handlebar_View_Retrive_Not_Defined_Culture_Key_SHOULD_Return_Local_Resource_Neutral_Value()
        {
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.CreateSpecificCulture("en-US")
                }
            );

            value.Should().NotBeNull();
            value.Should().Be("Neutral Page Title");
        }

        [Test]
        public void WHEN_Handlebar_View_Retreive_Generic_Language_SHOULD_Return_Local_Resource_French_Generic_Value()
        {
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.CreateSpecificCulture("fr-FR")
                }
            );

            value.Should().NotBeNull();
            value.Should().Be("Generic French Page Title");
        }

        [Test]
        public void WHEN_Handlebar_View_Retrieve_Culture_Specified_Key_SHOULD_Return_Local_Resource_French_Canadian_Value()
        {
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA")
                }
            );

            value.Should().NotBeNull();
            value.Should().Be("Québec Page Title");
        }

        [Test]
        public void WHEN_Handlebar_View_Retrieve_From_Non_Exising_ResourceFile_Return_Null()
        {
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest2",
                    Key         = "PageTitle",
                    CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA")
                }
            );

            value.Should().BeNull();
        }

        [Test]
        public void WHEN_Handlebar_View_Retrive_Not_Defined_Culture_And_Key_Does_Not_Exists_SHOULD_Return_Null()
        {
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "InvalidKey",
                    CultureInfo = CultureInfo.CreateSpecificCulture("en-US")
                }
            );

            value.Should().BeNull();
        }

        [Test]
        public void WHEN_Handlebar_View_Retreive_Generic_Language_And_Key_Does_Not_Exists_SHOULD_Return_Null()
        {
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "InvalidKey",
                    CultureInfo = CultureInfo.CreateSpecificCulture("fr-FR")
                }
            );

            value.Should().BeNull();
        }

        [Test]
        public void WHEN_Handlebar_View_Retrieve_Culture_Specified_And_Key_Does_Not_Exists_SHOULD_Return_Null()
        {
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category    = "ResxLocalizationTest",
                    Key         = "InvalidKey",
                    CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA")
                }
            );

            value.Should().BeNull();
        }

        [Test]
        [Timeout(10000)]
        public async Task WHEN_Using_AsyncToSync_SHOULD_Not_Deadlock()
        {
            //Start an async call in a nested context.. anything really that's the one that would endup beeing deadlocked
            await Task.Delay(1);

            //Start the sync to async call
            var value = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category = "ResxLocalizationTest",
                    Key = "PageTitle",
                    CultureInfo = CultureInfo.CreateSpecificCulture("fr-CA")
                }
            );

            //Nothing to assert really.. we just want to make sure the Timeout did not proc
            value.Should().NotBeNull();
            value.Should().Be("Québec Page Title");
        }

        [Test]
        [TestCase("ResxLocalizationTest", "ThisValueIsEmpty", "")]
        [TestCase("ResxLocalizationTest", "ThisValueIsWhitespace", "    ")]
        public void WHEN_Category_Key_Resolve_to_WhitespaceOrEmpty_Localized_Value_SHOULD_return_Localized_Value(string category, string key, string expectedValue)
        {
            var localizedString = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category = category,
                    Key = key,
                    CultureInfo = CultureInfo.GetCultureInfo("fr-CA")
                }
            );

            localizedString.Should().Be(expectedValue);
        }

        [Test]
        [TestCase("ResxLocalizationTest", "ThisValueHasSomeHtmlTag", "&gt;&gt;&lt;&lt; <<>>")]
        public void WHEN_Category_Key_Resolve_to_Localized_Value_With_Html_SHOULD_return_Localized_Value_Should_Not_Be_Encoded(string category, string key, string expectedValue)
        {
            var localizedString = _localizationProvider.GetLocalizedString(
                new GetLocalizedParam
                {
                    Category = category,
                    Key = key,
                    CultureInfo = CultureInfo.GetCultureInfo("fr-CA")
                }
            );

            localizedString.Should().Be(expectedValue);
        }

        
    }
}
