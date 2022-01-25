using System.Net;

namespace HookHook.Backend.Utilities
{
    public static class ClientAPI
    {
        public static async Task<T?> GetAsync<T>(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);

            Console.WriteLine("\nClient headers: \n");
            Console.Write(client.DefaultRequestHeaders);
            Console.WriteLine("\nResponse: \n");
            Console.Write(response);

            if (!response.IsSuccessStatusCode)
                return default;
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public static async Task<T?> PostAsync<T>(this HttpClient client, string url, HttpRequestMessage ?request = null)
        {
            var response = await client.PostAsync(url, request?.Content);

            if (!response.IsSuccessStatusCode)
                return default;
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}