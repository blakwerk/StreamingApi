namespace Streaming.Api.Tests.Data
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Provides fake but usable in-memory configuration for testing.
    /// </summary>
    public class FakeConfigurationProvider
    {
        public const string ValidKey = "ValidKey";
        public const string ValidValue = "ValidValue";

        /// <summary>
        /// Builds default configuration settings for tests.
        /// </summary>
        public static IConfiguration BuildDefaultConfiguration()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"TopLevelKey", "TopLevelValue"},
                {"SectionName:SomeKey", "SectionValue"},
                {ValidKey, ValidValue},
                //...populate as needed for tests
            };

            return BuildConfiguration(inMemorySettings);
        }

        /// <summary>
        /// Builds configuration settings for tests from keyvalue pairs
        /// </summary>
        public static IConfiguration BuildConfiguration(Dictionary<string, string> config)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            return configuration;
        }
    }
}