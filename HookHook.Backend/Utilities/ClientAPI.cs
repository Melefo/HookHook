using System.Net;
using System.Text.Json;

namespace HookHook.Backend.Utilities
{
    /// <summary>
    /// Wrapper arround HttpClient to parse result
    /// </summary>
    public static class ClientAPI
    {
        /// <summary>
        /// Do a GET request and return response class
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="client">Http client</param>
        /// <param name="url">URL to do request</param>
        /// <returns>Response</returns>
        public static async Task<T?> GetAsync<T>(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return default;
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;
            return await response.Content.ReadFromJsonAsync<T>();
        }

        /// <summary>
        /// Do a POST request and return response class
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="client">Http client</param>
        /// <param name="url">URL to do request</param>
        /// <param name="content">Body request</param>
        /// <returns>Response</returns>
        public static async Task<T?> PostAsync<T>(this HttpClient client, string url, HttpContent? content = null)
        {
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                return default;
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;
            return await response.Content.ReadFromJsonAsync<T>(new JsonSerializerOptions()
            {
                IncludeFields = true
            });
        }
    }
}