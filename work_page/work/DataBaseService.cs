using Supabase;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System.Collections.Generic;
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

        public async Task<List<Inventory>> GetInventoryAsync()
        {
            var response = await _supabaseClient
                .From<Inventory>()
                .Get();
            return response.Models;
        }

        public async Task<List<PetrolType>> GetPetrolTypesAsync()
        {
            var response = await _supabaseClient
                .From<PetrolType>()
                .Get();
            return response.Models;
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            await _supabaseClient
                .From<Inventory>()
                .Where(i => i.inventory_id == inventory.inventory_id)
                .Set(i => i.available, inventory.available)
                .Update();
        }
        public async Task UpdatePetrolTypeAsync(PetrolType petrolType)
        {
            await _supabaseClient
                .From<PetrolType>()
                .Update(petrolType);
        }
    }

    [Table("inventory")]
    public class Inventory : BaseModel
    {
        public int inventory_id { get; set; }
        public string petrol_name { get; set; } = string.Empty;
        [Column("available")]
        public double available { get; set; }
        public double max { get; set; }
    }

    [Table("petrol_type")]
    public class PetrolType : BaseModel
    {
        [PrimaryKey("id", false)]
        public int id { get; set; }
        public string petrol_name { get; set; } = string.Empty;
        [Column("price_per_liter")]
        public double price_per_liter { get; set; }
        [Column("tax")]
        public double tax { get; set; }
    }
}