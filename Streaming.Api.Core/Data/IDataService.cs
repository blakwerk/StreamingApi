namespace Streaming.Api.Core.Data
{
    using System.Threading.Tasks;

    public interface IDataService
    {
        /// <summary>
        /// Connects to the data source
        /// </summary>
        Task ConnectAsync();

        //TODO Add templated CRUD methods.
    }
}
