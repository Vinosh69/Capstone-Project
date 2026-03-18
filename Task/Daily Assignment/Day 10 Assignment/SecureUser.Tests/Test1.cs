using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureUser.Security;
using SecureUser.Services;

namespace SecureUserApp.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public void Password_Is_Hashed_And_Verified()
        {
            // Arrange
            string password = "test123";

            // Act
            var hash = PasswordHasher.HashPassword(password);
            bool isValid = PasswordHasher.Verify(password, hash);
            bool isInvalid = PasswordHasher.Verify("wrongpassword", hash);

            // Assert
            Assert.IsTrue(isValid);
            Assert.IsFalse(isInvalid);
        }

        [TestMethod]
        public void Encryption_And_Decryption_Works()
        {
            // Arrange
            string text = "HelloSecretData";

            // Act
            var encrypted = AesEncryption.Encrypt(text);
            var decrypted = AesEncryption.Decrypt(encrypted);

            // Assert
            Assert.AreEqual(text, decrypted);
        }

        [TestMethod]
        public void User_Can_Register_And_Login()
        {
            // Arrange
            var service = new UserService();

            // Act
            service.Register("user1", "pass1", "mysecret");
            bool loginResult = service.Login("user1", "pass1");

            // Assert
            Assert.IsTrue(loginResult);
        }
    }
}