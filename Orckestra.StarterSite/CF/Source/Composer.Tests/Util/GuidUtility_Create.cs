using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    public class GuidUtilityCreate
    {
        [Test]
        public void WHEN_name_is_null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            string name = null;

            //Act
            var ex = Assert.Throws<ArgumentNullException>(() => GuidUtility.Create(GetRandom.Guid(), name));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("name");
        }

        [Test]
        public void WHEN_name_is_null2_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            string name = null;

            //Act
            var ex = Assert.Throws<ArgumentNullException>(() => GuidUtility.Create(GetRandom.Guid(), name, GetRandom.Int()));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("name");
        }

        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(4)]
        [TestCase(2)]
        [TestCase(6)]
        [TestCase(999)]
        public void WHEN_version_is_not_3_or_5_SHOULD_throw_ArgumentOutOfRangeException(int version)
        {
            //Arrange
            string name = GetRandom.String(7);
            
            //Act
            var ex =
                Assert.Throws<ArgumentOutOfRangeException>(() => GuidUtility.Create(GetRandom.Guid(), name, version));

            //Assert
            ex.ParamName.Should().ContainEquivalentOf("version");
        }

        [TestCase("00000000-0000-0000-0000-000000000000", "")]
        [TestCase("FC2F7673-EF20-409F-B94A-A0EF860761E9", "Test1")]
        [TestCase("CF2A5569-738F-45ED-B0D4-8F84192A6736", "TestAdjhalkfsh")]
        [TestCase("615A8639-AF5C-4FBE-8115-E044E40D5FFF", "jkasghdkPPP")]
        [TestCase("E39F9C39-8A1E-4BBE-9DAF-111370E2BD03", "sadTest87445")]
        public void WHEN_parameters_ok_and_the_same_SHOULD_give_same_guids(string guid, string name)
        {
            //Arrange
            var uid = new Guid(guid);

            //Act
            var guid1 = GuidUtility.Create(uid, name);
            var guid2 = GuidUtility.Create(uid, name);

            //Assert
            guid1.Should().Be(guid2, "same parameters are expected to give the same result");
        }

        [TestCase("00000000-0000-0000-0000-000000000000", "", " ")]
        [TestCase("0A80C432-62D8-47FA-B7FF-8506676C717D", "Test1", "test1")]
        [TestCase(null, "YYzz--==/*-", "_YYzz--==/*-")]
        [TestCase(null, "--", "==")]
        [TestCase(null, "ABCDEFG", "abcdefg")]
        public void WHEN_guid_are_same_but_not_names_SHOULD_give_different_guids(string guid, string name1, string name2)
        {
            //Arrange
            var uid = guid == null ? GetRandom.Guid() : new Guid(guid);

            //Act
            var guid1 = GuidUtility.Create(uid, name1);
            var guid2 = GuidUtility.Create(uid, name2);

            //Assert
            guid1.Should().NotBe(guid2);
        }

        [TestCase("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000001", "")]
        [TestCase(null, null, "Test1")]
        [TestCase(null, null, "\r\n~**-@@8787")]
        [TestCase(null, null, "Te))00__**hola")]
        [TestCase(null, null, null)]
        public void WHEN_names_are_same_but_guid_are_different_SHOULD_give_different_guids(string guid1, string guid2,
            string name)
        {
            //Arrange
            var uid1 = guid1 == null ? GetRandom.Guid() : new Guid(guid1);
            var uid2 = guid2 == null ? GetRandom.Guid() : new Guid(guid2);
            var finalName = name ?? GetRandom.String(GetRandom.Int(1, 50));

            //Act
            var res1 = GuidUtility.Create(uid1, finalName);
            var res2 = GuidUtility.Create(uid2, finalName);

            //Assert
            res1.Should().NotBe(res2);
        }
    }
}
