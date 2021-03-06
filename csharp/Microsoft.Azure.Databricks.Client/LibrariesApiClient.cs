﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Client
{
    public class LibrariesApiClient : ApiClient, ILibrariesApi
    {
        public LibrariesApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<IDictionary<string, IEnumerable<LibraryFullStatus>>> AllClusterStatuses()
        {
            var result = await HttpGet<dynamic>(this.HttpClient, "libraries/all-cluster-statuses")
                .ConfigureAwait(false);

            if (PropertyExists(result, "statuses"))
            {
                return ((IEnumerable<dynamic>) result.statuses).ToDictionary(
                    d => (string) d.cluster_id.ToObject<string>(),
                    d => (IEnumerable<LibraryFullStatus>) d.library_statuses.ToObject<IEnumerable<LibraryFullStatus>>()
                );
            }

            return new Dictionary<string, IEnumerable<LibraryFullStatus>>();
        }

        public async Task<IEnumerable<LibraryFullStatus>> ClusterStatus(string clusterId)
        {
            var url = $"libraries/cluster-status?cluster_id={clusterId}";
            var result = await HttpGet<dynamic>(this.HttpClient, url).ConfigureAwait(false);
            return PropertyExists(result, "library_statuses")
                ? result.library_statuses.ToObject<IEnumerable<LibraryFullStatus>>()
                : Enumerable.Empty<LibraryFullStatus>();
        }

        public async Task Install(string clusterId, IEnumerable<Library> libraries)
        {
            if (libraries == null)
            {
                return;
            }

            var array = libraries as Library[] ?? libraries.ToArray();

            if (!array.Any())
            {
                return;
            }

            var request = new {cluster_id = clusterId, libraries = array };
            await HttpPost(this.HttpClient, "libraries/install", request).ConfigureAwait(false);
        }

        public async Task Uninstall(string clusterId, IEnumerable<Library> libraries)
        {
            if (libraries == null)
            {
                return;
            }

            var array = libraries as Library[] ?? libraries.ToArray();

            if (!array.Any())
            {
                return;
            }

            var request = new { cluster_id = clusterId, libraries = array };
            await HttpPost(this.HttpClient, "libraries/uninstall", request).ConfigureAwait(false);
        }
    }
}