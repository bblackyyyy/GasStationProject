using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Data;
using Npgsql;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace work
{
    internal class DataBaseService
    {
        private readonly HttpClient _httpClient;

        // Zamień na swój projekt Supabase
        private const string BaseUrl = "https://aovmwvcrszjxevuiilzz.supabase.co";
        private const string TableName = "Transaction"; // np. users
        private const string ApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFvdm13dmNyc3pqeGV2dWlpbHp6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDE1MjI4MTUsImV4cCI6MjA1NzA5ODgxNX0.Agf_enVje9oOlIQy3FPuHrMkBo8DPBDEMlob9K4YgMo";

        public DataBaseService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
            _httpClient.DefaultRequestHeaders.Add("apikey", ApiKey);
        }

        public async Task GetDataAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{TableName}?select=*");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Dane z Supabase:");
                    Console.WriteLine(json);
                }
                else
                {
                    Console.WriteLine($"Błąd: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas połączenia z Supabase: " + ex.Message);
            }
        }
    }
}
