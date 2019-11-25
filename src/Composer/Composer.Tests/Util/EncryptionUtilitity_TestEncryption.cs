using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class EncryptionUtilitity_TestEncryption
    {
        [Test]
        public void WHEN_data_is_encrypted_SHOULD_return_a_base_64_string()
        {
            //Arrange
            var encryptionUtility = new EncryptionUtility();

            //Act
            var result = encryptionUtility.Encrypt("some data");

            //Assert
            result.Should().NotBeEmpty();
            result.Should().MatchRegex("^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$");
        }

        [Test]
        public void WHEN_null_is_encrypted_SHOULD_throw_an_argumentnullexception()
        {
            //Arrange
            var encryptionUtility = new EncryptionUtility();

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => encryptionUtility.Encrypt(null));
        }
    }
}
