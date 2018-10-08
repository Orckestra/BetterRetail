using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Tests.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Tests.Providers
{
    [TestFixture]
    public class ProxyFactory_Create
    {
        public AutoMocker Container { get; set; }

        [SetUp]
        public void SetUp()
        {
            Container = new AutoMocker();

            var registry = new ViewModelMetadataRegistry();
            registry.LoadViewModelMetadataInAssemblyOf(GetType().Assembly);
            Container.Use<IViewModelMetadataRegistry>(registry);
        }

        [Test]
        public void WHEN_parameter_type_defines_a_method_SHOULD_throw_InvalidOperationException()
        {
            // Arrange
            var viewModel = new ExtendedViewModel();

            // Act
            var exception =
                Assert.Throws<InvalidOperationException>(
                    () => ExtensionTypeProxyFactory.Default.Create<IViewModelExtensionWithMethod>(viewModel));

            // Assert
            exception.Should().NotBeNull();
        }

        [Test]
        public void WHEN_parameter_type_does_is_not_IExtensionOf_SHOULD_throw_InvalidOperationException()
        {
            // Arrange
            var viewModel = new ExtendedViewModel();

            // Act
            var exception =
                Assert.Throws<InvalidOperationException>(
                    () => ExtensionTypeProxyFactory.Default.Create<INonExtensionViewModel>(viewModel));

            // Assert
            exception.Should().NotBeNull();
        }

        [Test]
        public void WHEN_parameter_type_does_is_not_interface_SHOULD_throw_InvalidOperationException()
        {
            // Arrange
            var viewModel = new ExtendedViewModel();

            // Act
            var exception =
                Assert.Throws<InvalidOperationException>(() => ExtensionTypeProxyFactory.Default.Create<ExtendedViewModel>(viewModel));

            // Assert
            exception.Should().NotBeNull();
        }

        [Test]
        public void WHEN_parameter_type_is_missing_property_getter_SHOULD_throw_InvalidOperationException()
        {
            // Arrange
            var viewModel = new ExtendedViewModel();

            // Act
            var exception =
                Assert.Throws<InvalidOperationException>(
                    () => ExtensionTypeProxyFactory.Default.Create<IViewModelExtensionWithMissingPropertyGetter>(viewModel));

            // Assert
            exception.Should().NotBeNull();
        }

        [Test]
        public void WHEN_parameter_type_is_missing_property_setter_SHOULD_throw_InvalidOperationException()
        {
            // Arrange
            var viewModel = new ExtendedViewModel();

            // Act
            var exception =
                Assert.Throws<InvalidOperationException>(
                    () => ExtensionTypeProxyFactory.Default.Create<IViewModelExtensionWithMissingPropertySetter>(viewModel));

            // Assert
            exception.Should().NotBeNull();
        }

        [Test]
        public void
            WHEN_valid_extension_and_edit_custom_property_SHOULD_store_value_in_extended_model_property_bag()
        {
            // Arrange
            var viewModel = new ExtendedViewModel();
            var extendedViewModel = ExtensionTypeProxyFactory.Default.Create<IExtensionViewModel>(viewModel);
            var customPropertyValue = 12345;

            // Act
            extendedViewModel.CustomProperty = customPropertyValue;

            // Assert
            extendedViewModel.CustomProperty.ShouldBeEquivalentTo(customPropertyValue);
            viewModel.Bag.ContainsKey("CustomProperty").Should().BeTrue();
            viewModel.Bag["CustomProperty"].Should().Be(customPropertyValue);
        }

        [Test]
        public void
            WHEN_parameter_type_is_valid_SHOULD_return_an_instance_of_an_implementation_of_ExtensionOf_view_model_type()
        {
            // Arrange
            var basePropertyValue = "test";
            var viewModel = new ExtendedViewModel { BaseProperty = basePropertyValue };

            // Act
            var extendedViewModel = ExtensionTypeProxyFactory.Default.Create<IExtensionViewModel>(viewModel);

            // Assert
            extendedViewModel.Should().NotBeNull();
            extendedViewModel.Model.Should().NotBeNull();
            extendedViewModel.Model.Should().BeAssignableTo<ExtendedViewModel>();
            extendedViewModel.Model.Should().BeSameAs(viewModel);
            extendedViewModel.Model.BaseProperty.ShouldBeEquivalentTo(basePropertyValue);
        }

        [Test]
        public void WHEN_viewModel_is_null_SHOULD_throw_ArgumentNullException()
        {
            // Arrange
            ExtendedViewModel viewModel = null;

            // Act
            var exception =
                Assert.Throws<ArgumentNullException>(() => ExtensionTypeProxyFactory.Default.Create<IExtensionViewModel>(viewModel));

            // Assert
            exception.Should().NotBeNull();
        }

        [Test(Description = "Tests that generating many proxy in a racing condition does not make the type creation fail.")]
        public void WHEN_viewmodel_creation_is_racing_SHOULD_create_type_once()
        {
            //Arrange
            var typeCreationFunc = new Func<IExtensionViewModel>(() =>
            {
                var vm = new ExtendedViewModel
                {
                    BaseProperty = GetRandom.String(10)
                };

                var extension = ExtensionTypeProxyFactory.Default.Create<IExtensionViewModel>(vm);
                return extension;
            });

            //Act
            var tasks = new List<Task<IExtensionViewModel>>();
            for (var i = 0; i < 100; i++)
            {
                var t = Task.Factory.StartNew(typeCreationFunc);
                tasks.Add(t);
            }
            Task.WaitAll(tasks.Cast<Task>().ToArray());

            //Assert
            tasks.Any(t => t.IsFaulted).Should().BeFalse();
        }

        [Test]
        public void WHEN_base_type_is_not_in_bag_SHOULD_return_base_type_default()
        {
            //Arrange
            var obj = new
            {
                BaseProperty = GetRandom.String(7),
                PropertyBag = new PropertyBag()
            };

            var mapper = Container.CreateInstance<ViewModelMapper>();
            var vm = mapper.MapTo<ExtendedViewModel>(obj, CultureInfo.InvariantCulture);
            var proxy = vm.AsExtensionModel<IExtensionViewModel>();

            //Act
            int? intValue = null;
            bool? boolValue = null;
            Assert.DoesNotThrow(()=> intValue = proxy.CustomProperty);
            Assert.DoesNotThrow(()=> boolValue = proxy.CustomBool);

            //Assert
            intValue.Should().Be(default(int));
            boolValue.Should().Be(default(bool));
        }
    }
}