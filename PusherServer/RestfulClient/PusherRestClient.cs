using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PusherServer.RestfulClient
{
    /// <summary>
    /// A client for the Pusher REST requests
    /// </summary>
    public class PusherRestClient : IPusherRestClient
    {
        private readonly string _libraryName;
        private readonly string _version;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructs a new instance of the PusherRestClient
        /// </summary>
        /// <param name="baseAddress">The base address of the Pusher API as a URI formatted string</param>
        /// <param name="libraryName"></param>
        /// <param name="version"></param>
        public PusherRestClient(string baseAddress, string libraryName, Version version) : this(new Uri(baseAddress), libraryName, version)
        {}

        /// <summary>
        /// Constructs a new instance of the PusherRestClient
        /// </summary>
        /// <param name="baseAddress">The base address of the Pusher API</param>
        /// <param name="libraryName">The name of the Pusher Library</param>
        /// <param name="version">The version of the Pusher library</param>
        public PusherRestClient(Uri baseAddress, string libraryName, Version version)
        {
            _httpClient = new HttpClient { BaseAddress = baseAddress };
            _libraryName = libraryName;
            _version = version.ToString(3);
        }

        /// <summary>
        /// Execute a REST request to the Pusher API asynchronously
        /// </summary>
        /// <param name="pusherRestRequest">The request to execute</param>
        /// <returns>The response received from Pusher</returns>
        public async Task<GetResult2<T>> ExecuteAsync<T>(IPusherRestRequest pusherRestRequest)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Pusher-Library-Name", _libraryName);
            _httpClient.DefaultRequestHeaders.Add("Pusher-Library-Version", _version);

            if (pusherRestRequest.Method == PusherMethod.GET)
            {
                var response = await _httpClient.GetAsync(pusherRestRequest.ResourceUri);
                var responseContent = await response.Content.ReadAsStringAsync();

                return new GetResult2<T>(response, responseContent);
            }

            if (pusherRestRequest.Method == PusherMethod.POST)
            {
                var content = new StringContent(pusherRestRequest.GetContentAsJsonString(), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(pusherRestRequest.ResourceUri, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return new GetResult2<T>(response, responseContent);
            }

            return null;
        }
    }
}
