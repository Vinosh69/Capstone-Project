using SecureUser.Services;
using SecureUser.Services;
using Serilog;
using System;

namespace SecureUser.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("app.log")
                .CreateLogger();

            try
            {
                var service = new UserService();

                System.Console.WriteLine("=== Secure User App Demo ===");

                // Register user
                service.Register("vinosh", "vinosh123", "MySecretData");
                System.Console.WriteLine("User registered successfully.");

                // Login
                bool loginOk = service.Login("vinosh", "vinosh123");
                System.Console.WriteLine("Login success: " + loginOk);

                // Decrypt secret
                var secret = service.GetDecryptedSecret("vinosh");
                System.Console.WriteLine("Decrypted secret: " + secret);

                System.Console.WriteLine("Check app.log file for logs.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application crashed");
                System.Console.WriteLine("An error occurred. Check logs.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            System.Console.WriteLine("Press any key to exit...");
            System.Console.ReadKey();
        }
    }
}