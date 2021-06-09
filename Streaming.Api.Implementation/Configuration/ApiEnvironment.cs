namespace Streaming.Api.Implementation.Configuration
{
    using Streaming.Api.Core.Configuration;

    public class ApiEnvironment : IApiEnvironment
    {
        /// <inheritdoc />
        public string DatabaseConnection { get; } = "ConnectionStrings:DefaultConnection";
    }
}
