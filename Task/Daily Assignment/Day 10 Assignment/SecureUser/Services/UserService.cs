using SecureUser.Models;
using SecureUser.Security;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureUser.Services
{
    public class UserService
    {
        private readonly List<User> _users = new List<User>();

        public void Register(string username, string password, string secret)
        {
            try
            {
                var hashed = PasswordHasher.HashPassword(password);
                var encryptedSecret = AesEncryption.Encrypt(secret);

                _users.Add(new User
                {
                    Username = username,
                    HashedPassword = hashed,
                    EncryptedSecret = encryptedSecret
                });

                Log.Information("User {Username} registered successfully at {Time}", username, DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during registration at {Time}", DateTime.Now);
                throw;
            }
        }

        public bool Login(string username, string password)
        {
            try
            {
                var user = _users.FirstOrDefault(u => u.Username == username);
                if (user == null) return false;

                bool ok = PasswordHasher.Verify(password, user.HashedPassword);

                if (ok)
                    Log.Information("User {Username} logged in successfully at {Time}", username, DateTime.Now);
                else
                    Log.Warning("Failed login attempt for {Username} at {Time}", username, DateTime.Now);

                return ok;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during login at {Time}", DateTime.Now);
                return false;
            }
        }

        public string GetDecryptedSecret(string username)
        {
            var user = _users.First(u => u.Username == username);
            return AesEncryption.Decrypt(user.EncryptedSecret);
        }
    }
}
