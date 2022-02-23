using System.Net;
using System.Text.Json;

namespace HookHook.Backend.Utilities
{
    public static class ClientAPI
    {
        public static async Task<T?> GetAsync<T>(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return default;
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public static async Task<T?> PostAsync<T>(this HttpClient client, string url, HttpContent? content = null)
        {
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                return default;
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return await response.Content.ReadFromJsonAsync<T>(new JsonSerializerOptions()
            {
                IncludeFields = true
            });
        }
    }
}