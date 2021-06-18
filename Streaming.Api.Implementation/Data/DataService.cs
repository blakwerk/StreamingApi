namespace Streaming.Api.Implementation.Data
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Streaming.Api.Core.Configuration;
    using Streaming.Api.Core.Data;
    using System.Threading.Tasks;
    using Streaming.Api.Models;

    internal class DataService : IDataService
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;
        private readonly IApiEnvironment _apiEnvironment;

        private readonly object hashLock = new ();

        //private readonly HashSet<string> processedTweets;
        private readonly ConcurrentDictionary<string, int> processedDomains;

        private readonly ConcurrentDictionary<string, int> processedTweets;

        public DataService(
            ILogger<DataService> log, 
            IConfiguration config,
            IApiEnvironment apiEnvironment)
        {
            _log = log;
            _config = config;
            _apiEnvironment = apiEnvironment;

            //this.processedTweets = new HashSet<string>();
            this.processedTweets = new ConcurrentDictionary<string, int>();
            this.processedDomains = new ConcurrentDictionary<string, int>();
        }
        
        /// <inheritdoc />
        public Task ConnectAsync()
        {
            var settingKey = _apiEnvironment.DatabaseConnection;

            try
            {
                if (string.IsNullOrWhiteSpace(settingKey))
                {
                    throw new ArgumentException("Database connection key is invalid!");
                }

                var connectionString = _config.GetValue<string>(settingKey);

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ArgumentException("Database connection string is invalid!");
                }

                _log.LogInformation($"Connecting to data store {connectionString}");

                // This is where one would actually connect to the database if it were
                // a real data store. Possibly directly, possibly via a connection
                // multiplexer.

                _log.LogInformation($"Connected to data store {connectionString}");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, $"Unable to connect to datastore: {settingKey}");
                throw;
            }
        }

        /// <inheritdoc />
        public Task UpsertTweetAsync(IStreamedTweet tweet)
        {
            //bool newlyAdded;

            //lock (this.hashLock)
            //{
            //    newlyAdded = this.processedTweets.Add(tweet.Id);
            //}

            //if(!newlyAdded)
            //{
            //    // for the purposes of this data exercise, do not double-process tweets
            //    return Task.CompletedTask;
            //}

            if (this.processedTweets.ContainsKey(tweet.Id))
            {
                // for the purposes of this data exercise, do not double-process tweets
                return Task.CompletedTask;
            }

            this.processedTweets.AddOrUpdate(tweet.Id, 0, (_, _) => 0);
            
            this.UpsertDomains(tweet.Uris);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<int> GetTweetProcessedCountAsync()
        {
            //int processedCount;

            //lock (this.hashLock)
            //{
            //    processedCount = this.processedTweets.Count;
            //}

            //return Task.FromResult(processedCount);
            return Task.FromResult(this.processedTweets.Count);
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetTopDomainsAsync(int takeCount)
        {
            var topDomainsKvpSnapshot = this.processedDomains.ToList();

            topDomainsKvpSnapshot.Sort((kvp1, kvp2) => kvp1.Value.CompareTo(kvp2.Value));

            var topDomains = topDomainsKvpSnapshot.Select(kvp => kvp.Key).Take(takeCount);

            return Task.FromResult(topDomains);
        }

        private void UpsertDomains(IEnumerable<Uri> uris)
        {
            foreach (var uri in uris)
            {
                this.processedDomains.AddOrUpdate(uri.Host, 0, (s, i) => i+1);
            }
        }
    }
}
