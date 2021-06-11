namespace Streaming.Api.Implementation.Configuration
{
    using Streaming.Api.Core.Configuration;

    /// <summary>
    /// A helper class which removes magic strings and provides semantic
    /// meaning to configuration keys.
    /// </summary>
    public class ApiEnvironment : IApiEnvironment
    {
        /// <inheritdoc />
        public string DatabaseConnection { get; } = "ConnectionStrings:DefaultConnection";

        /// <inheritdoc />
        public string V2ApiBaseUri { get; } = "StaticDataFeedEndpoints:V2Api";
    }
}
