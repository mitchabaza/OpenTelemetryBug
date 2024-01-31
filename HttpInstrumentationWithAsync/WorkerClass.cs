using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpInstrumentationWithAsync
{
    public class WorkerClass
    {
        private static readonly ActivitySource ActivitySource = new(nameof(Program));
        private readonly HttpClient _httpClient;

        public WorkerClass()
        {
            _httpClient = new HttpClient();
        }


        public async Task StartAsync()
        {
            await Task.WhenAll(CallPostManEcho("10"),CallPostManEcho("20"),CallPostManEcho("30")).ConfigureAwait(true);
        }

        public void StartSync()
        {
           
            _ =  CallPostManEcho("10").Result;
            _ =  CallPostManEcho("20").Result;
            _ =  CallPostManEcho("30").Result;
            
        }

      
        private async Task<string> CallPostManEcho(string value)
        {
            using var activity = ActivitySource.StartActivity($"{nameof(CallPostManEcho)}-{value}");
            {
                var response = await CallRestApi($"https://postman-echo.com/get?Id={value}");

                return response;
            }
        }


        private async Task<string> CallRestApi(string uri)
        {
            using var httpResponse = await _httpClient.GetAsync(uri);

            httpResponse.EnsureSuccessStatusCode();

            if (httpResponse.Content is not null && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
            {
                var contentStream = await httpResponse.Content.ReadAsStreamAsync();

                using var streamReader = new StreamReader(contentStream);
                string json = await streamReader.ReadToEndAsync();
                return json;
            }

            Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");

            return null;
        }
    }
}