using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Database.DbUp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Based on CI/CD set up can either go for storing/building connectionstring from Environment variables or appsettings file.
                var config = new ConfigurationBuilder()
                        .AddEnvironmentVariables()
                        .AddUserSecrets<Program>()
                        .Build();

                var currentEnvironment = config.GetSection("ASPNETCORE_ENVIRONMENT").Value;
                var connectionString = config.GetSection("ConnectionString").Value;
                
                if (currentEnvironment.IsNullOrEmpty() || currentEnvironment!.Equals("Development"))
                {
                    EnsureDatabase.For.SqlDatabase(connectionString!);
                }

                var upgradeEngine = DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

                var result = upgradeEngine.PerformUpgrade();

                if (result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Job done!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(result.Error);
                    Console.ResetColor();
                }

                //Connection string can be either read from as is as a string from appsettings/env variables or it can be constructed using connectionstring builder class.


            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("EXCEPTION");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
    }
}
