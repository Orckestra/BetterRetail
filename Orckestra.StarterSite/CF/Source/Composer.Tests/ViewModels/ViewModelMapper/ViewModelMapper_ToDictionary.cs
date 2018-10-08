using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ViewModels.ViewModelMapper
{
    [TestFixture(Category = "ViewModelMapper")]
	public class ViewModelMapperToDictionary : BaseTest
	{
        public Composer.ViewModels.ViewModelMapper ViewModelMapper { get; set; }

        public override void SetUp()
        {
            base.SetUp();

            RegisterSpecificDependencies();
            ViewModelMapper = Container.CreateInstance<Composer.ViewModels.ViewModelMapper>();
        }

		[Test]
		public void WHEN_Passing_Null_SHOULD_Throw_An_ArgumentNullException()
		{
            // Act 
		    Action nullViewModel = () => ViewModelMapper.ToDictionary(null);

            // Assert
		    nullViewModel.ShouldThrow<ArgumentNullException>();
		}

		[Test]
		public void WHEN_Using_Valid_Parameters_SHOULD_Create_A_Dictionary()
		{
            // Arrange
		    ViewModelMetadataRegistry.Current = Container.Get<IViewModelMetadataRegistry>();
		    var validProductViewModel = TestProductViewModelFactory.CreateValid();

            // Act
            var result = ViewModelMapper.ToDictionary(validProductViewModel);

            // Assert
		    result.Should().NotBeNull();
		    result.Should().HaveCount(16);
		    result["Brand"].Should().Be("AmericanTire");
            result["Price"].Should().Be(9.99);
            result["Name"].Should().Be("Chair");
            result["Colour"].Should().Be("Brown");
		    result["CustomProperty"].Should().Be(5);
		    result["MappedViewModelBagProperty"].Should().Be("I'm a mapped field!");
		    result["Category"].Should().NotBeNull();
		    result["Category"].Should().BeAssignableTo<IDictionary<string, object>>();
		    result.Should().ContainKey("JsonContext");

            var category = (IDictionary<string, object>)result["Category"];
		    category["Title"].Should().Be("Sports");
            category["Id"].Should().Be(42);
		    category["CustomName"].Should().Be("Custom sports");
		}

        private void RegisterSpecificDependencies()
        {
            var registry = new ViewModelMetadataRegistry();
            registry.LoadViewModelMetadataInAssemblyOf<ViewModelMapperToDictionary>();
            Container.Use<IViewModelMetadataRegistry>(registry);
        }
	}
}