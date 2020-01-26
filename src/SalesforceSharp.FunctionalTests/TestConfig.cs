using System;
using System.Configuration;
using TestSharp;

namespace SalesforceSharp.FunctionalTests
{
    using System.IO;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Test's configurations.
    /// <remarks>
    /// PLEASE, DO NOT USE PRODUCTION DATA FOR TESTS. SANDBOX EXISTS FOR THIS PURPOSE.
    /// </remarks>
    /// </summary>
    public static class TestConfig
    {
        static TestConfig()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json").Build();
                
                TokenRequestEndpointUrl = configuration["TokenRequestEndpointUrl"];
                ClientId = configuration["ClientId"];
                ClientSecret = configuration["ClientSecret"];
                Username = configuration["Username"];
                Password = configuration["Password"];
                ObjectName = configuration["ObjectName"];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Please, check the Salesforce.FunctionalTests' App.config and define the test configurations.", ex);
            }

            if (String.IsNullOrWhiteSpace(TokenRequestEndpointUrl)
            || String.IsNullOrWhiteSpace(ClientId)
            || String.IsNullOrWhiteSpace(ClientSecret)
            || String.IsNullOrWhiteSpace(Username)
            || String.IsNullOrWhiteSpace(Password)
            || String.IsNullOrWhiteSpace(ObjectName))
            {
                throw new InvalidOperationException("Please, check the Salesforce.FunctionalTests' App.config and define ALL the test configurations.");
            }           
        }

        public static string TokenRequestEndpointUrl { get; set; }
        public static string ClientId { get; set; }
        public static string ClientSecret { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string ObjectName { get; set; }
    }
}
