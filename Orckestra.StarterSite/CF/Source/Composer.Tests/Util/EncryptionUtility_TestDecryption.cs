using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Tests.Util
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class EncryptionUtilitity_TestDecryption
    {
        [Test]
        public void WHEN_data_is_encrypted_SHOULD_return_the_same_value_when_decrypted()
        {
            //Arrange
            var encryptionUtility = new EncryptionUtility();

            //Act
            var expected = "some data";
            var encrypted = encryptionUtility.Encrypt(expected);
            var result = encryptionUtility.Decrypt(encrypted);

            //Assert
            result.Should().Be(expected, "because this was the value originally encrypted.");
        }

        [Test]
        public void WHEN_null_is_decrypted_SHOULD_throw_an_argumentnullexception()
        {
            //Arrange
            var encryptionUtility = new EncryptionUtility();

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => encryptionUtility.Decrypt(null));
        }

        [Test]
        public void WHEN_data_is_decrypted_that_is_not_a_base64_string_SHOULD_throw_an_formatexception()
        {
            //Arrange
            var encryptionUtility = new EncryptionUtility();

            //Act
            //Assert
            Assert.Throws<FormatException>(() => encryptionUtility.Decrypt("I'm not a base 64 string."));
        }
    }
}
