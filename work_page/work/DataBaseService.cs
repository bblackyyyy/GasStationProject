using Supabase;
using System.Threading.Tasks;

namespace work
{
    internal class DataBaseService
    {
        private readonly Supabase.Client _supabaseClient;

        private const string SupabaseUrl = "https://aovmwvcrszjxevuiilzz.supabase.co";
        private const string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFvdm13dmNyc3pqeGV2dWlpbHp6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDE1MjI4MTUsImV4cCI6MjA1NzA5ODgxNX0.Agf_enVje9oOlIQy3FPuHrMkBo8DPBDEMlob9K4YgMo";

        public DataBaseService()
        {
            _supabaseClient = new Supabase.Client(SupabaseUrl, SupabaseKey);
            _supabaseClient.InitializeAsync().Wait();
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(email, password);
                return session != null && !string.IsNullOrEmpty(session.AccessToken);
            }
            catch
            {
                return false;
            }
        }
    }
}