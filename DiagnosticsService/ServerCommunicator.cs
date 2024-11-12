using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

public static class ServerCommunicator
{
    private static HttpClient _httpClient = new();
    
    static ServerCommunicator()
    {
        _httpClient.BaseAddress = new Uri("..."); // Setting endpoint
    }
    
    /// <summary>
    /// Sends a POST request with the diagnostics data to the server asynchronously.
    /// </summary>
    /// <param name="path">The server endpoint path.</param>
    /// <param name="payload">Diagnostics data to send.</param>
    internal static async void SendDiagnosticsToServer(string path, object payload)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, path);
            // Headers if needed
            // request.Headers.TryAddWithoutValidation("auth", "...");

            var stringPayload = JsonConvert.SerializeObject(payload);
            request.Content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
        
            var responseMessage = await _httpClient.SendAsync(request);
            // Handle response
        }
        catch (Exception e)
        {
            // Handle exception
        }
    }
}